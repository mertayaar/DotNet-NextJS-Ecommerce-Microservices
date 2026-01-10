using Ecommerce.Images.Models;
using Ecommerce.Images.Settings;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace Ecommerce.Images.Services
{
    
    
    
    
    
    
    
    
    
    
    public class GoogleCloudImageUploadService : IImageUploadService
    {
        private readonly StorageClient _storageClient;
        private readonly GoogleCloudSettings _settings;
        private readonly ILogger<GoogleCloudImageUploadService> _logger;

        
        private static readonly Dictionary<string, byte[]> ImageMagicBytes = new()
        {
            { "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
            { "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            { "image/gif", new byte[] { 0x47, 0x49, 0x46 } },
            { "image/webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } } 
        };

        public GoogleCloudImageUploadService(
            IOptions<GoogleCloudSettings> settings,
            ILogger<GoogleCloudImageUploadService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            
            if (!string.IsNullOrEmpty(_settings.CredentialsPath))
            {
                
                _storageClient = StorageClient.Create(
                    Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(_settings.CredentialsPath));
                _logger.LogInformation("GCS client initialized with service account from {Path}", _settings.CredentialsPath);
            }
            else
            {
                
                
                _storageClient = StorageClient.Create();
                _logger.LogInformation("GCS client initialized with Application Default Credentials");
            }
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder = "products")
        {
            try
            {
                
                var (isValid, error) = await ValidateImageAsync(file);
                if (!isValid)
                {
                    _logger.LogWarning("Image validation failed: {Error}", error);
                    return ImageUploadResult.Failed(error!);
                }

                
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var objectName = string.IsNullOrEmpty(folder) 
                    ? uniqueFileName 
                    : $"{folder.Trim('/')}/{uniqueFileName}";

                
                var contentType = file.ContentType;
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = GetContentType(fileExtension);
                }

                _logger.LogInformation(
                    "Uploading image: {OriginalName} -> {ObjectName} ({ContentType}, {Size} bytes)",
                    file.FileName, objectName, contentType, file.Length);

                
                using var stream = file.OpenReadStream();
                var uploadedObject = await _storageClient.UploadObjectAsync(
                    _settings.BucketName,
                    objectName,
                    contentType,
                    stream,
                    new UploadObjectOptions());

                var publicUrl = _settings.GetPublicUrl(objectName);

                _logger.LogInformation(
                    "Image uploaded successfully: {ObjectName} -> {Url}",
                    objectName, publicUrl);

                return ImageUploadResult.Succeeded(
                    url: publicUrl,
                    fileName: file.FileName,
                    objectName: objectName,
                    fileSize: file.Length,
                    contentType: contentType);
            }
            catch (Google.GoogleApiException ex)
            {
                _logger.LogError(ex, "GCS API error during upload: {Message}", ex.Message);
                return ImageUploadResult.Failed($"Cloud storage error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during image upload");
                return ImageUploadResult.Failed("An unexpected error occurred during upload");
            }
        }

        public async Task<bool> DeleteImageAsync(string objectName)
        {
            try
            {
                _logger.LogInformation("Deleting image: {ObjectName}", objectName);
                
                await _storageClient.DeleteObjectAsync(_settings.BucketName, objectName);
                
                _logger.LogInformation("Image deleted successfully: {ObjectName}", objectName);
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Image not found for deletion: {ObjectName}", objectName);
                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ObjectName}", objectName);
                return false;
            }
        }

        public async Task<(bool IsValid, string? Error)> ValidateImageAsync(IFormFile file)
        {
            
            if (file == null || file.Length == 0)
            {
                return (false, "No file provided");
            }

            
            if (file.Length > _settings.MaxFileSizeBytes)
            {
                var maxSizeMb = _settings.MaxFileSizeBytes / (1024 * 1024);
                return (false, $"File size exceeds maximum allowed size of {maxSizeMb}MB");
            }

            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                return (false, $"File extension '{extension}' is not allowed. Allowed: {string.Join(", ", _settings.AllowedExtensions)}");
            }

            
            if (!string.IsNullOrEmpty(file.ContentType) && 
                !_settings.AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return (false, $"Content type '{file.ContentType}' is not allowed");
            }

            
            var isValidMagicBytes = await ValidateMagicBytesAsync(file);
            if (!isValidMagicBytes)
            {
                return (false, "File content does not match a valid image format");
            }

            return (true, null);
        }

        
        
        
        
        private async Task<bool> ValidateMagicBytesAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var headerBytes = new byte[8]; 
                var bytesRead = await stream.ReadAsync(headerBytes, 0, headerBytes.Length);

                if (bytesRead < 3)
                {
                    return false;
                }

                
                foreach (var (mimeType, magicBytes) in ImageMagicBytes)
                {
                    if (bytesRead >= magicBytes.Length)
                    {
                        var match = true;
                        for (int i = 0; i < magicBytes.Length; i++)
                        {
                            if (headerBytes[i] != magicBytes[i])
                            {
                                match = false;
                                break;
                            }
                        }

                        if (match)
                        {
                            
                            if (mimeType == "image/webp" && bytesRead >= 12)
                            {
                                
                                stream.Seek(0, SeekOrigin.Begin);
                                var webpHeader = new byte[12];
                                await stream.ReadAsync(webpHeader, 0, 12);
                                
                                
                                if (webpHeader[8] == 0x57 && webpHeader[9] == 0x45 && 
                                    webpHeader[10] == 0x42 && webpHeader[11] == 0x50)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating magic bytes");
                return false;
            }
        }

        private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}

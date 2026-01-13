using Ecommerce.WebUI.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Ecommerce.WebUI.Services.ImageUpload
{
    public class GcsImageUploadService : IImageUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly ILogger<GcsImageUploadService> _logger;

        public GcsImageUploadService(HttpClient httpClient, IOptions<ServiceApiSettings> serviceApiSettings, ILogger<GcsImageUploadService> logger)
        {
            _httpClient = httpClient;
            _serviceApiSettings = serviceApiSettings.Value;
            _logger = logger;
        }

        public async Task<string?> UploadAsync(IFormFile file, string folder = "products")
        {
            try
            {
                _logger.LogInformation("Starting image upload for {FileName}", file.FileName);

                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.FileName);

                var requestUrl = $"{_serviceApiSettings.OcelotUrl}/services/images/GoogleCloudImageUpload/upload?folder={folder}";
                _logger.LogInformation("Sending POST request to {Url}", requestUrl);

                var response = await _httpClient.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ImageUploadResponse>(responseContent);
                    _logger.LogInformation("Upload successful. URL: {Url}", result?.Url);
                    return result?.Url;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Upload failed with status code {StatusCode}. Content: {Content}", response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during image upload");
                return null;
            }
        }
    }

    public class ImageUploadResponse
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? FileName { get; set; }
        public string? Error { get; set; }
    }
}

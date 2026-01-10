namespace Ecommerce.Images.Settings
{
    
    
    
    
    public class GoogleCloudSettings
    {
        
        
        
        public string ProjectId { get; set; } = string.Empty;

        
        
        
        public string BucketName { get; set; } = string.Empty;

        
        
        
        
        public string? CredentialsPath { get; set; }

        
        
        
        public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;

        
        
        
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        
        
        
        public string[] AllowedMimeTypes { get; set; } = 
        { 
            "image/jpeg", 
            "image/png", 
            "image/webp", 
            "image/gif" 
        };

        
        
        
        
        
        public string? CdnBaseUrl { get; set; }

        
        
        
        public string GetPublicUrl(string objectName)
        {
            if (!string.IsNullOrEmpty(CdnBaseUrl))
            {
                return $"{CdnBaseUrl.TrimEnd('/')}/{objectName}";
            }
            return $"https://storage.googleapis.com/{BucketName}/{objectName}";
        }
    }
}

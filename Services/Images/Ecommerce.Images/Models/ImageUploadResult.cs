namespace Ecommerce.Images.Models
{
    
    
    
    public record ImageUploadResult
    {
        public bool Success { get; init; }
        public string? Url { get; init; }
        public string? FileName { get; init; }
        public string? ObjectName { get; init; }
        public long? FileSize { get; init; }
        public string? ContentType { get; init; }
        public string? Error { get; init; }

        public static ImageUploadResult Succeeded(string url, string fileName, string objectName, long fileSize, string contentType)
            => new()
            {
                Success = true,
                Url = url,
                FileName = fileName,
                ObjectName = objectName,
                FileSize = fileSize,
                ContentType = contentType
            };

        public static ImageUploadResult Failed(string error)
            => new()
            {
                Success = false,
                Error = error
            };
    }

    
    
    
    public class ImageUploadResponse
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? FileName { get; set; }
        public string? Error { get; set; }
    }
}

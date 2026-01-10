using Microsoft.AspNetCore.Http;

namespace Ecommerce.WebUI.Services.ImageUpload
{
    public interface IImageUploadService
    {
        Task<string?> UploadAsync(IFormFile file, string folder = "products");
    }
}

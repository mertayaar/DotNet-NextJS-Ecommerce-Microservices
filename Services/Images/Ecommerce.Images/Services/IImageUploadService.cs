using Ecommerce.Images.Models;

namespace Ecommerce.Images.Services
{
    
    
    
    
    public interface IImageUploadService
    {
        
        
        
        
        
        
        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string folder = "products");

        
        
        
        
        
        Task<bool> DeleteImageAsync(string objectName);

        
        
        
        
        
        Task<(bool IsValid, string? Error)> ValidateImageAsync(IFormFile file);
    }
}

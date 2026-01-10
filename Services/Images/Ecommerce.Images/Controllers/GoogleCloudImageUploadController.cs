using Ecommerce.Images.Models;
using Ecommerce.Images.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Images.Controllers
{
    
    
    
    
    
    
    
    
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleCloudImageUploadController : ControllerBase
    {
        private readonly IImageUploadService _imageUploadService;
        private readonly ILogger<GoogleCloudImageUploadController> _logger;

        public GoogleCloudImageUploadController(
            IImageUploadService imageUploadService,
            ILogger<GoogleCloudImageUploadController> logger)
        {
            _imageUploadService = imageUploadService;
            _logger = logger;
        }

        
        
        
        
        
        
        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] 
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ImageUploadResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadImage(
            IFormFile file,
            [FromQuery] string folder = "products")
        {
            _logger.LogInformation(
                "Upload request received - File: {FileName}, Size: {Size}, Folder: {Folder}",
                file?.FileName ?? "null", file?.Length ?? 0, folder);

            if (file == null || file.Length == 0)
            {
                return BadRequest(new ImageUploadResponse
                {
                    Success = false,
                    Error = "No file provided"
                });
            }

            
            if (!IsValidFolderName(folder))
            {
                return BadRequest(new ImageUploadResponse
                {
                    Success = false,
                    Error = "Invalid folder name"
                });
            }

            var result = await _imageUploadService.UploadImageAsync(file, folder);

            if (!result.Success)
            {
                return BadRequest(new ImageUploadResponse
                {
                    Success = false,
                    Error = result.Error
                });
            }

            return Ok(new ImageUploadResponse
            {
                Success = true,
                Url = result.Url,
                FileName = result.ObjectName
            });
        }

        
        
        
        
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteImage([FromQuery] string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return BadRequest(new { error = "Object name is required" });
            }

            
            if (objectName.Contains("..") || objectName.StartsWith("/"))
            {
                return BadRequest(new { error = "Invalid object name" });
            }

            _logger.LogInformation("Delete request for: {ObjectName}", objectName);

            var success = await _imageUploadService.DeleteImageAsync(objectName);

            if (!success)
            {
                return StatusCode(500, new { error = "Failed to delete image" });
            }

            return Ok(new { success = true, message = "Image deleted successfully" });
        }

        
        
        
        [HttpPost("validate")]
        [RequestSizeLimit(10_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { valid = false, error = "No file provided" });
            }

            var (isValid, error) = await _imageUploadService.ValidateImageAsync(file);

            return Ok(new
            {
                valid = isValid,
                error = error,
                fileName = file.FileName,
                size = file.Length,
                contentType = file.ContentType
            });
        }

        
        
        
        [AllowAnonymous]
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "images", timestamp = DateTime.UtcNow });
        }

        
        
        
        private static bool IsValidFolderName(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return true; 
            }

            
            if (folder.Contains("..") || folder.Contains("//") || folder.StartsWith("/"))
            {
                return false;
            }

            
            return folder.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '/');
        }
    }
}
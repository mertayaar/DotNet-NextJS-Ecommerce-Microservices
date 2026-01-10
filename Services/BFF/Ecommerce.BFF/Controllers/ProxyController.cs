using Ecommerce.BFF.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BFF.Controllers
{
    
    
    
    
    
    
    
    
    
    [ApiController]
    [Route("api")]
    public class ProxyController : ControllerBase
    {
        private readonly ITokenStore _tokenStore;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProxyController> _logger;
        private readonly IConfiguration _configuration;
        private const string SESSION_COOKIE_NAME = "bff_session";
        private readonly string _gatewayUrl;
        private readonly string _imagesServiceUrl;

        public ProxyController(
            ITokenStore tokenStore,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ProxyController> logger)
        {
            _tokenStore = tokenStore;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _gatewayUrl = _configuration["ServiceApiSettings:Ocelot:BaseUrl"]!;
            _imagesServiceUrl = _configuration["ServiceApiSettings:Images:BaseUrl"]!;
        }

        
        
        

        
        
        
        
        [HttpGet("product/{**path}")]
        public async Task<IActionResult> GetProduct(string path)
        {
            return await ProxyRequest(HttpMethod.Get, $"/services/catalog/{path}", requiresAuth: false);
        }

        
        
        

        
        
        
        
        [HttpGet("cart/{**path}")]
        public async Task<IActionResult> GetCart(string path)
        {
            return await ProxyRequest(HttpMethod.Get, $"/services/cart/{path}", requiresAuth: true);
        }

        
        
        
        
        [HttpPost("cart/{**path}")]
        public async Task<IActionResult> PostCart(string path)
        {
            var body = await ReadRequestBodyAsync();
            return await ProxyRequest(HttpMethod.Post, $"/services/cart/{path}", body, requiresAuth: true);
        }

        
        
        
        
        [HttpDelete("cart/{**path}")]
        public async Task<IActionResult> DeleteCart(string path)
        {
            return await ProxyRequest(HttpMethod.Delete, $"/services/cart/{path}", requiresAuth: true);
        }

        
        
        
        
        [HttpGet("order/{**path}")]
        public async Task<IActionResult> GetOrder(string path)
        {
            return await ProxyRequest(HttpMethod.Get, $"/services/order/{path}", requiresAuth: true);
        }

        
        
        
        
        [HttpPost("order/{**path}")]
        public async Task<IActionResult> PostOrder(string path)
        {
            var body = await ReadRequestBodyAsync();
            return await ProxyRequest(HttpMethod.Post, $"/services/order/{path}", body, requiresAuth: true);
        }

        
        
        
        
        [HttpGet("discount/{**path}")]
        public async Task<IActionResult> GetDiscount(string path)
        {
            return await ProxyRequest(HttpMethod.Get, $"/services/discount/{path}", requiresAuth: true);
        }

        
        
        
        
        [HttpGet("cargo/{**path}")]
        public async Task<IActionResult> GetCargo(string path)
        {
            return await ProxyRequest(HttpMethod.Get, $"/services/cargo/{path}", requiresAuth: true);
        }

        
        
        
        
        [HttpPost("cargo/{**path}")]
        public async Task<IActionResult> PostCargo(string path)
        {
            var body = await ReadRequestBodyAsync();
            return await ProxyRequest(HttpMethod.Post, $"/services/cargo/{path}", body, requiresAuth: true);
        }

        [HttpGet("auth/{**path}")]
public async Task<IActionResult> AuthProxy(string path)
{
    return await ProxyRequest(HttpMethod.Get, $"/auth/{path}", requiresAuth: true);
}

        
        
        

        
        
        
        
        
        
        
        
        
        [HttpPost("images/upload")]
        [RequestSizeLimit(10_000_000)] 
        public async Task<IActionResult> UploadImage([FromQuery] string folder = "products")
        {
            try
            {
                
                var sessionId = Request.Cookies[SESSION_COOKIE_NAME];
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("Image upload without session cookie");
                    return Unauthorized(new { error = "Not authenticated" });
                }

                var tokens = await _tokenStore.GetTokensAsync(sessionId);
                if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                {
                    _logger.LogWarning("Session {SessionId} not found or expired", sessionId);
                    return Unauthorized(new { error = "Session expired" });
                }

                
                if (tokens.ExpiresAt < DateTime.UtcNow.AddMinutes(-1))
                {
                    _logger.LogInformation("Token expired for session {SessionId}", sessionId);
                    return Unauthorized(new { error = "Token expired", requiresRefresh = true });
                }

                
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromMinutes(2); 

                
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

                
                if (!Request.HasFormContentType)
                {
                    return BadRequest(new { error = "Request must be multipart/form-data" });
                }

                var form = await Request.ReadFormAsync();
                var file = form.Files.FirstOrDefault();

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "No file provided" });
                }

                
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType ?? "application/octet-stream");
                content.Add(streamContent, "file", file.FileName);

                
                var imagesServiceUrl = $"{_imagesServiceUrl}/api/GoogleCloudImageUpload/upload?folder={Uri.EscapeDataString(folder)}";
                
                _logger.LogInformation("Forwarding image upload to {Url}", imagesServiceUrl);

                var response = await client.PostAsync(imagesServiceUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Image upload response: {StatusCode}", response.StatusCode);

                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during image upload proxy");
                return StatusCode(500, new { error = "Image upload failed", message = ex.Message });
            }
        }

        
        
        
        
        [HttpDelete("images")]
        public async Task<IActionResult> DeleteImage([FromQuery] string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return BadRequest(new { error = "objectName is required" });
            }

            try
            {
                
                var sessionId = Request.Cookies[SESSION_COOKIE_NAME];
                if (string.IsNullOrEmpty(sessionId))
                {
                    return Unauthorized(new { error = "Not authenticated" });
                }

                var tokens = await _tokenStore.GetTokensAsync(sessionId);
                if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                {
                    return Unauthorized(new { error = "Session expired" });
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

                var imagesServiceUrl = $"{_imagesServiceUrl}/api/GoogleCloudImageUpload?objectName={Uri.EscapeDataString(objectName)}";
                
                var response = await client.DeleteAsync(imagesServiceUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during image delete proxy");
                return StatusCode(500, new { error = "Image deletion failed" });
            }
        }


        
        
        

        private async Task<IActionResult> ProxyRequest(
            HttpMethod method,
            string path,
            string? body = null,
            bool requiresAuth = false)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                
                
                
                if (requiresAuth)
                {
                    var sessionId = Request.Cookies[SESSION_COOKIE_NAME];

                    if (string.IsNullOrEmpty(sessionId))
                    {
                        _logger.LogWarning("Proxy request without session cookie");
                        return Unauthorized(new { error = "Not authenticated" });
                    }

                    
                    var tokens = await _tokenStore.GetTokensAsync(sessionId);

                    if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                    {
                        _logger.LogWarning("Session {SessionId} not found or expired", sessionId);
                        return Unauthorized(new { error = "Session expired" });
                    }

                    
                    if (tokens.ExpiresAt < DateTime.UtcNow.AddMinutes(-1))
                    {
                        _logger.LogInformation("Token expired for session {SessionId}", sessionId);
                        return Unauthorized(new { error = "Token expired", requiresRefresh = true });
                    }

                    
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

                    _logger.LogDebug("Added Bearer token for session {SessionId}", sessionId);
                }

                
                
                
                var gatewayUrl = $"{_gatewayUrl}{path}";

                
                if (Request.QueryString.HasValue)
                {
                    gatewayUrl += Request.QueryString.Value;
                }

                var request = new HttpRequestMessage(method, gatewayUrl);

                
                if (!string.IsNullOrEmpty(body) && method != HttpMethod.Get && method != HttpMethod.Delete)
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                
                ForwardSafeHeaders(request);

                _logger.LogInformation("{Method} {Path} -> {GatewayUrl}", method, path, gatewayUrl);

                
                
                
                var response = await client.SendAsync(request);

                
                
                
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Response {StatusCode} from {GatewayUrl}", response.StatusCode, gatewayUrl);

                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during proxy request to {Path}", path);
                return StatusCode(503, new { error = "Service unavailable", message = ex.Message });
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout during proxy request to {Path}", path);
                return StatusCode(504, new { error = "Gateway timeout" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during proxy request to {Path}", path);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        
        
        

        private async Task<string?> ReadRequestBodyAsync()
        {
            if (Request.ContentLength > 0)
            {
                using var reader = new StreamReader(Request.Body);
                return await reader.ReadToEndAsync();
            }
            return null;
        }

        private void ForwardSafeHeaders(HttpRequestMessage request)
        {
            
            var safeHeaders = new[] { "Accept", "Accept-Language", "User-Agent", "X-Request-Id" };

            foreach (var header in safeHeaders)
            {
                if (Request.Headers.TryGetValue(header, out var value))
                {
                    request.Headers.TryAddWithoutValidation(header, value.ToArray());
                }
            }
        }
    }
}

using Ecommerce.BFF.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Ecommerce.BFF.Controllers
{
    [ApiController]
    [Route("addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStore _tokenStore;
        private readonly ILogger<AddressesController> _logger;
        private readonly IConfiguration _configuration;
        private const string SESSION_COOKIE_NAME = "bff_session";
        private readonly string _cargoServiceUrl;

        public AddressesController(
            IHttpClientFactory httpClientFactory, 
            ITokenStore tokenStore,
            IConfiguration configuration,
            ILogger<AddressesController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStore = tokenStore;
            _configuration = configuration;
            _logger = logger;
            _cargoServiceUrl = _configuration["ServiceApiSettings:Cargo:BaseUrl"]!;
        }

        private async Task<(HttpClient? client, string? userId)> GetAuthenticatedClient()
        {
            var sessionId = Request.Cookies[SESSION_COOKIE_NAME];
            if (string.IsNullOrEmpty(sessionId)) return (null, null);

            var tokens = await _tokenStore.GetTokensAsync(sessionId);
            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken)) return (null, null);

            
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokens.AccessToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            return (client, userId);
        }

        
        
        
        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var (client, userId) = await GetAuthenticatedClient();
            if (client == null || string.IsNullOrEmpty(userId)) 
                return Unauthorized(new { error = "Not authenticated" });

            try
            {
                var response = await client.GetAsync($"{_cargoServiceUrl}/api/cargocustomers/user/{userId}/addresses");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get addresses");
                return StatusCode(500, new { error = "Failed to fetch addresses" });
            }
        }

        
        
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var (client, userId) = await GetAuthenticatedClient();
            if (client == null) return Unauthorized(new { error = "Not authenticated" });

            try
            {
                var response = await client.GetAsync($"{_cargoServiceUrl}/api/cargocustomers/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get address {Id}", id);
                return StatusCode(500, new { error = "Failed to fetch address" });
            }
        }

        
        
        
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] JsonElement addressData)
        {
            var (client, userId) = await GetAuthenticatedClient();
            if (client == null || string.IsNullOrEmpty(userId)) 
                return Unauthorized(new { error = "Not authenticated" });

            try
            {
                
                var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(addressData.GetRawText()) 
                    ?? new Dictionary<string, object>();
                dataDict["userCustomerId"] = userId;

                var jsonContent = new StringContent(JsonSerializer.Serialize(dataDict), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_cargoServiceUrl}/api/cargocustomers", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create address");
                return StatusCode(500, new { error = "Failed to create address" });
            }
        }

        
        
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] JsonElement addressData)
        {
            var (client, userId) = await GetAuthenticatedClient();
            if (client == null || string.IsNullOrEmpty(userId)) 
                return Unauthorized(new { error = "Not authenticated" });

            try
            {
                
                var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(addressData.GetRawText()) 
                    ?? new Dictionary<string, object>();
                dataDict["cargoCustomerId"] = id;
                dataDict["userCustomerId"] = userId;

                var jsonContent = new StringContent(JsonSerializer.Serialize(dataDict), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_cargoServiceUrl}/api/cargocustomers", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true });
                }

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update address {Id}", id);
                return StatusCode(500, new { error = "Failed to update address" });
            }
        }

        
        
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var (client, userId) = await GetAuthenticatedClient();
            if (client == null) return Unauthorized(new { error = "Not authenticated" });

            try
            {
                var response = await client.DeleteAsync($"{_cargoServiceUrl}/api/cargocustomers/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true });
                }

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete address {Id}", id);
                return StatusCode(500, new { error = "Failed to delete address" });
            }
        }

        
        
        
        [HttpPost("{id}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var (client, userId) = await GetAuthenticatedClient();
            if (client == null || string.IsNullOrEmpty(userId)) 
                return Unauthorized(new { error = "Not authenticated" });

            try
            {
                var response = await client.PostAsync($"{_cargoServiceUrl}/api/cargocustomers/{id}/set-default?userId={userId}", null);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true });
                }

                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set default address {Id}", id);
                return StatusCode(500, new { error = "Failed to set default address" });
            }
        }
    }
}

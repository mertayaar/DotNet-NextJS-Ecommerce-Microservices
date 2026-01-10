using Ecommerce.BFF.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Ecommerce.BFF.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStore _tokenStore;
        private readonly IConfiguration _configuration;
        private const string SESSION_COOKIE_NAME = "bff_session";
        private readonly string _identityServiceUrl;

        public UsersController(IHttpClientFactory httpClientFactory, ITokenStore tokenStore, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStore = tokenStore;
            _configuration = configuration;
            _identityServiceUrl = _configuration["ServiceApiSettings:Identity:BaseUrl"]!;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] object updateData)
        {
            var sessionId = Request.Cookies[SESSION_COOKIE_NAME];
            if (string.IsNullOrEmpty(sessionId)) return Unauthorized("Not authenticated");

            var tokens = await _tokenStore.GetTokensAsync(sessionId);
            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken)) return Unauthorized("Session expired");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var content = new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_identityServiceUrl}/api/users/update", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { success = true, message = "Profile updated successfully" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new { success = false, message = "Update failed", details = errorContent });
        }

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] object passwordData)
        {
            var sessionId = Request.Cookies[SESSION_COOKIE_NAME];
            if (string.IsNullOrEmpty(sessionId)) return Unauthorized("Not authenticated");

            var tokens = await _tokenStore.GetTokensAsync(sessionId);
            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken)) return Unauthorized("Session expired");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var content = new StringContent(JsonSerializer.Serialize(passwordData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_identityServiceUrl}/api/users/change-password", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { success = true, message = "Password changed successfully" });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new { success = false, message = "Password change failed", details = errorContent });
        }
    }
}

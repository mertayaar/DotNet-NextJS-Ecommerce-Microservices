using Ecommerce.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ecommerce.WebUI.Handlers
{
    
    
    
    
    
    
    public class ResourceOwnerPasswordTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string BFF_SESSION_COOKIE = "webui_bff_session";

        public ResourceOwnerPasswordTokenHandler(
            IHttpContextAccessor httpContextAccessor, 
            IIdentityService identityService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
            var accessToken = await GetAccessTokenFromBff();
            
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await _identityService.GetRefreshToken();
                if (refreshed)
                {
                    accessToken = await GetAccessTokenFromBff();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        response = await base.SendAsync(request, cancellationToken);
                    }
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Session expired. Please log in again.");
            }

            return response;
        }

        private async Task<string?> GetAccessTokenFromBff()
        {
            var sessionId = GetBffSessionId();
            if (string.IsNullOrEmpty(sessionId))
            {
                return null;
            }

            try
            {
                var bffUrl = _configuration["ServiceApiSettings:BffUrl"] ?? "http://localhost:5500";
                var client = _httpClientFactory.CreateClient();

                var requestBody = new { sessionId = sessionId };
                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync($"{bffUrl}/auth/get-token", content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                return tokenResponse?.AccessToken;
            }
            catch
            {
                return null;
            }
        }

        private string? GetBffSessionId()
        {
            
            if (_httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(BFF_SESSION_COOKIE, out var sessionId))
            {
                return sessionId;
            }

            
            return _httpContextAccessor.HttpContext.User.FindFirst("bff_session")?.Value;
        }

        private class TokenResponse
        {
            [JsonPropertyName("accessToken")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("expiresAt")]
            public DateTimeOffset ExpiresAt { get; set; }
        }
    }
}

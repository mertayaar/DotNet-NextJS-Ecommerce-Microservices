using Ecommerce.DtoLayer.IdentityDtos.LoginDtos;
using Ecommerce.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ecommerce.WebUI.Services.Concrete
{
    
    
    
    
    
    
    
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private const string BFF_SESSION_COOKIE = "webui_bff_session";

        public IdentityService(
            HttpClient httpClient, 
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<bool> GetRefreshToken()
        {
            var sessionId = GetBffSessionId();
            if (string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            try
            {
                var bffUrl = _configuration["ServiceApiSettings:BffUrl"] ?? "http://localhost:5500";
                var request = new { sessionId = sessionId };
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{bffUrl}/auth/get-token", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                if (tokenData != null && !string.IsNullOrEmpty(tokenData.AccessToken))
                {
                    
                    var authResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync();
                    if (authResult.Succeeded)
                    {
                        var properties = authResult.Properties;
                        properties.StoreTokens(new[]
                        {
                            new AuthenticationToken { Name = "access_token", Value = tokenData.AccessToken }
                        });
                        await _httpContextAccessor.HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            authResult.Principal!,
                            properties);
                    }
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SignIn(SignInDto signInDto)
        {
            var bffUrl = _configuration["ServiceApiSettings:BffUrl"] ?? "http://localhost:5500";
            
            var loginRequest = new
            {
                username = signInDto.Username,
                password = signInDto.Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{bffUrl}/auth/admin-login", content);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<AdminLoginResponse>(responseContent);

            if (loginResponse == null || !loginResponse.Success || string.IsNullOrEmpty(loginResponse.SessionId))
            {
                return false;
            }

            
            _httpContextAccessor.HttpContext!.Response.Cookies.Append(
                BFF_SESSION_COOKIE,
                loginResponse.SessionId,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, 
                    SameSite = SameSiteMode.Lax,
                    MaxAge = TimeSpan.FromDays(30)
                });

            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResponse.User?.Sub ?? ""),
                new Claim(ClaimTypes.Name, loginResponse.User?.Name ?? signInDto.Username),
                new Claim(ClaimTypes.Email, loginResponse.User?.Email ?? ""),
                new Claim("bff_session", loginResponse.SessionId)
            };

            
            if (loginResponse.User?.Roles != null)
            {
                foreach (var role in loginResponse.User.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = loginResponse.ExpiresAt
            };

            
            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = loginResponse.SessionId }
            });

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            return true;
        }

        private string? GetBffSessionId()
        {
            
            if (_httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(BFF_SESSION_COOKIE, out var sessionId))
            {
                return sessionId;
            }

            
            return _httpContextAccessor.HttpContext.User.FindFirst("bff_session")?.Value;
        }

        
        private class AdminLoginResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("sessionId")]
            public string SessionId { get; set; } = string.Empty;

            [JsonPropertyName("user")]
            public UserInfo? User { get; set; }

            [JsonPropertyName("expiresAt")]
            public DateTimeOffset ExpiresAt { get; set; }
        }

        private class UserInfo
        {
            [JsonPropertyName("sub")]
            public string? Sub { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("email")]
            public string? Email { get; set; }

            [JsonPropertyName("roles")]
            public List<string>? Roles { get; set; }
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

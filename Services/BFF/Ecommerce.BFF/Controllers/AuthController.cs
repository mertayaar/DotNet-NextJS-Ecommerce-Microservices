using Ecommerce.BFF.Models;
using Ecommerce.BFF.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ecommerce.BFF.Controllers
{
    
    
    
    
    
    
    
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenStore _tokenStore;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        private const string SESSION_COOKIE_NAME = "bff_session";
        private const string CLIENT_ID = "ecommerce_bff";
        private readonly string _identityServerUrl;
        private readonly string _clientBaseUrl;

        public AuthController(
            ITokenStore tokenStore,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _tokenStore = tokenStore;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _identityServerUrl = _configuration["ServiceApiSettings:Identity:BaseUrl"]!;
            _clientBaseUrl = _configuration["ServiceApiSettings:Client:BaseUrl"]!;
        }

        
        
        
        
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string returnUrl = "/")
        {
            
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var state = GenerateRandomString(32);

            
            var tempCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, 
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(10), 
                Path = "/"
            };

            Response.Cookies.Append("pkce_verifier", codeVerifier, tempCookieOptions);
            Response.Cookies.Append("oauth_state", state, tempCookieOptions);
            Response.Cookies.Append("return_url", returnUrl, tempCookieOptions);

            
            var authUrl = $"{_identityServerUrl}/connect/authorize?" +
                $"client_id={CLIENT_ID}&" +
                $"response_type=code&" +
                $"scope=openid profile email roles offline_access catalog.read catalog.write cart.manage order.read order.write order.admin discount.read discount.write message.read message.write review.read review.write cargo.read cargo.write image.read image.upload payment.process IdentityServerApi&" +
                $"redirect_uri={Uri.EscapeDataString($"{_clientBaseUrl}/backend/auth/callback")}&" +
                $"state={state}&" +
                $"code_challenge={codeChallenge}&" +
                $"code_challenge_method=S256";

            return Redirect(authUrl);
        }

        
        
        
        
        
        
        
        
        [HttpPost("login-credentials")]
        public async Task<IActionResult> LoginWithCredentials([FromBody] LoginRequest request)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Username and password are required" });
                }

                
                var returnUrl = ValidateReturnUrl(request.ReturnUrl);

                
                var tokenResponse = await ExchangePasswordForTokens(request.Username, request.Password);

                if (tokenResponse == null)
                {
                    
                    await Task.Delay(Random.Shared.Next(100, 500)); 
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                
                _logger.LogInformation("Token response - AccessToken: {HasAccess}, IdToken: {HasId}, RefreshToken: {HasRefresh}",
                    !string.IsNullOrEmpty(tokenResponse.AccessToken),
                    !string.IsNullOrEmpty(tokenResponse.IdToken),
                    !string.IsNullOrEmpty(tokenResponse.RefreshToken));

                
                var sessionId = GenerateSecureSessionId();
                _logger.LogInformation("Generated session ID: {SessionId}", sessionId);

                
                var tokenSet = new TokenSet
                {
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    IdToken = tokenResponse.IdToken,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    Scope = tokenResponse.Scope
                };

                await _tokenStore.StoreTokensAsync(sessionId, tokenSet);
                _logger.LogInformation("Tokens stored for session {SessionId}", sessionId);

                
                
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,  
                    Secure = false,    
                    SameSite = SameSiteMode.Lax,  
                    MaxAge = TimeSpan.FromDays(30),
                    Path = "/",
                    IsEssential = true
                };

                Response.Cookies.Append(SESSION_COOKIE_NAME, sessionId, cookieOptions);
                _logger.LogInformation("Session cookie set: {CookieName}={SessionId}", SESSION_COOKIE_NAME, sessionId);

                return Ok(new
                {
                    success = true,
                    returnUrl = returnUrl,
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        
        
        
        
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { message = "Username, email, and password are required" });
                }

                
                var client = _httpClientFactory.CreateClient();
                var registerData = new
                {
                    username = request.Username,
                    email = request.Email,
                    name = request.Name,
                    surname = request.Surname,
                    password = request.Password
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(registerData),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"{_identityServerUrl}/api/registers", content);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Registration successful" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { message = "Registration failed", details = errorContent });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }


        
        
        
        
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                return BadRequest("Missing code or state parameter");
            }

            
            var storedVerifier = Request.Cookies["pkce_verifier"];
            var storedState = Request.Cookies["oauth_state"];
            var returnUrl = Request.Cookies["return_url"] ?? "/";

            
            if (state != storedState)
            {
                return BadRequest("Invalid state parameter - possible CSRF attack");
            }

            if (string.IsNullOrEmpty(storedVerifier))
            {
                return BadRequest("Missing PKCE verifier");
            }

            try
            {
                
                var client = _httpClientFactory.CreateClient();
                var tokenRequest = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "client_id", CLIENT_ID },
                    { "redirect_uri", $"{_clientBaseUrl}/backend/auth/callback" },
                    { "code_verifier", storedVerifier } 
                };

                var tokenResponse = await client.PostAsync(
                    $"{_identityServerUrl}/connect/token",
                    new FormUrlEncodedContent(tokenRequest)
                );

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                    return BadRequest($"Token exchange failed: {errorContent}");
                }

                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(tokenJson);

                
                var tokenSet = new TokenSet
                {
                    AccessToken = tokenData["access_token"].GetString(),
                    RefreshToken = tokenData.ContainsKey("refresh_token") ? tokenData["refresh_token"].GetString() : null,
                    IdToken = tokenData.ContainsKey("id_token") ? tokenData["id_token"].GetString() : null,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenData["expires_in"].GetInt32()),
                    Scope = tokenData.ContainsKey("scope") ? tokenData["scope"].GetString() : null
                };

                
                var sessionId = Guid.NewGuid().ToString();

                
                await _tokenStore.StoreTokensAsync(sessionId, tokenSet);

                
                var sessionCookieOptions = new CookieOptions
                {
                    HttpOnly = true,     
                    Secure = false,      
                    SameSite = SameSiteMode.Lax, 
                    MaxAge = TimeSpan.FromDays(30),
                    Path = "/",
                    IsEssential = true
                };

                Response.Cookies.Append(SESSION_COOKIE_NAME, sessionId, sessionCookieOptions);

                
                Response.Cookies.Delete("pkce_verifier");
                Response.Cookies.Delete("oauth_state");
                Response.Cookies.Delete("return_url");

                
                return Redirect($"{_clientBaseUrl}{returnUrl}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Authentication error: {ex.Message}");
            }
        }

        
        
        
        
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            try 
            {
                var sessionId = Request.Cookies[SESSION_COOKIE_NAME];
                _logger.LogInformation("GetUser called. SessionId present: {HasSession}, Value: {SessionId}", 
                    !string.IsNullOrEmpty(sessionId), sessionId);

                if (string.IsNullOrEmpty(sessionId))
                {
                    return Unauthorized(new { error = "Not authenticated" });
                }

                var tokens = await _tokenStore.GetTokensAsync(sessionId);
                _logger.LogInformation("GetUser: Tokens retrieved - HasTokens: {HasTokens}, HasAccess: {HasAccess}, HasId: {HasId}",
                    tokens != null,
                    tokens != null && !string.IsNullOrEmpty(tokens.AccessToken),
                    tokens != null && !string.IsNullOrEmpty(tokens.IdToken));

                
                if (tokens == null || (string.IsNullOrEmpty(tokens.IdToken) && string.IsNullOrEmpty(tokens.AccessToken)))
                {
                    _logger.LogWarning("GetUser: No valid tokens found for session.");
                    return Unauthorized(new { error = "Session expired" });
                }

                
                UserInfo userInfo;
                if (!string.IsNullOrEmpty(tokens.IdToken))
                {
                    userInfo = ExtractUserInfoFromIdToken(tokens.IdToken);
                    _logger.LogInformation("GetUser: Extracted info from IdToken for user {User}", userInfo.Name ?? userInfo.Sub);
                }
                else if (!string.IsNullOrEmpty(tokens.AccessToken))
                {
                    
                    userInfo = ExtractUserInfoFromIdToken(tokens.AccessToken);
                    _logger.LogInformation("GetUser: Extracted info from AccessToken for user {User}", userInfo.Name ?? userInfo.Sub);
                }
                else
                {
                    _logger.LogWarning("GetUser: No token available to extract user info.");
                    return Unauthorized(new { error = "Session expired" });
                }

                return Ok(new { success = true, user = userInfo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUser");
                return StatusCode(500, new { error = "Internal error checking auth status" });
            }
        }

        
        
        
        
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var sessionId = Request.Cookies[SESSION_COOKIE_NAME];

            if (string.IsNullOrEmpty(sessionId))
            {
                return Unauthorized(new { error = "Not authenticated" });
            }

            var tokens = await _tokenStore.GetTokensAsync(sessionId);

            if (tokens == null || string.IsNullOrEmpty(tokens.RefreshToken))
            {
                return Unauthorized(new { error = "No refresh token available" });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var refreshRequest = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", CLIENT_ID },
                    { "refresh_token", tokens.RefreshToken }
                };

                var tokenResponse = await client.PostAsync(
                    $"{_identityServerUrl}/connect/token",
                    new FormUrlEncodedContent(refreshRequest)
                );

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    await _tokenStore.RemoveTokensAsync(sessionId);
                    Response.Cookies.Delete(SESSION_COOKIE_NAME);
                    return Unauthorized(new { error = "Token refresh failed" });
                }

                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(tokenJson);

                
                tokens.AccessToken = tokenData["access_token"].GetString();
                if (tokenData.ContainsKey("refresh_token"))
                {
                    tokens.RefreshToken = tokenData["refresh_token"].GetString();
                }
                tokens.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenData["expires_in"].GetInt32());

                await _tokenStore.UpdateTokensAsync(sessionId, tokens);

                return Ok(new { success = true, message = "Token refreshed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Refresh error: {ex.Message}");
            }
        }

        
        
        
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var sessionId = Request.Cookies[SESSION_COOKIE_NAME];

            if (!string.IsNullOrEmpty(sessionId))
            {
                
                await _tokenStore.RemoveTokensAsync(sessionId);
            }

            
            Response.Cookies.Delete(SESSION_COOKIE_NAME);

            return Ok(new { success = true, message = "Logged out" });
        }

        
        
        
        
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var sessionId = Request.Cookies[SESSION_COOKIE_NAME];

            if (string.IsNullOrEmpty(sessionId))
            {
                return Ok(new { isAuthenticated = false });
            }

            var tokens = await _tokenStore.GetTokensAsync(sessionId);

            return Ok(new 
            { 
                isAuthenticated = tokens != null && !string.IsNullOrEmpty(tokens.AccessToken),
                tokenExpired = tokens?.ExpiresAt < DateTime.UtcNow
            });
        }

        
        
        

        
        
        
        
        
        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { success = false, message = "Username and password are required" });
                }

                
                var tokenResponse = await ExchangePasswordForTokens(request.Username, request.Password);

                if (tokenResponse == null)
                {
                    await Task.Delay(Random.Shared.Next(100, 500));
                    return Unauthorized(new { success = false, message = "Invalid credentials" });
                }

                
                var sessionId = GenerateSecureSessionId();

                
                var tokenSet = new TokenSet
                {
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    IdToken = tokenResponse.IdToken,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    Scope = tokenResponse.Scope
                };

                await _tokenStore.StoreTokensAsync(sessionId, tokenSet);

                
                var userInfo = ExtractUserInfoFromIdToken(tokenResponse.AccessToken);

                _logger.LogInformation("Admin login successful for user: {User}", userInfo.Name ?? request.Username);

                
                return Ok(new
                {
                    success = true,
                    sessionId = sessionId,
                    user = userInfo,
                    expiresAt = tokenSet.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin login error");
                return StatusCode(500, new { success = false, message = "An error occurred during login" });
            }
        }

        
        
        
        
        
        [HttpPost("get-token")]
        public async Task<IActionResult> GetToken([FromBody] GetTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.SessionId))
            {
                return Unauthorized(new { error = "No session" });
            }

            var tokens = await _tokenStore.GetTokensAsync(request.SessionId);

            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
            {
                return Unauthorized(new { error = "Session expired" });
            }

            
            if (tokens.ExpiresAt < DateTime.UtcNow.AddMinutes(5) && !string.IsNullOrEmpty(tokens.RefreshToken))
            {
                
                try
                {
                    var refreshedToken = await RefreshAccessToken(tokens.RefreshToken);
                    if (refreshedToken != null)
                    {
                        tokens.AccessToken = refreshedToken.AccessToken;
                        tokens.ExpiresAt = DateTime.UtcNow.AddSeconds(refreshedToken.ExpiresIn);
                        if (!string.IsNullOrEmpty(refreshedToken.RefreshToken))
                        {
                            tokens.RefreshToken = refreshedToken.RefreshToken;
                        }
                        await _tokenStore.UpdateTokensAsync(request.SessionId, tokens);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to refresh token");
                }
            }

            return Ok(new
            {
                accessToken = tokens.AccessToken,
                expiresAt = tokens.ExpiresAt
            });
        }

        
        
        
        
        [HttpPost("admin-logout")]
        public async Task<IActionResult> AdminLogout([FromBody] LogoutRequest request)
        {
            if (!string.IsNullOrEmpty(request.SessionId))
            {
                await _tokenStore.RemoveTokensAsync(request.SessionId);
            }

            return Ok(new { success = true });
        }

        private async Task<TokenResponse?> RefreshAccessToken(string refreshToken)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", CLIENT_ID },
                { "refresh_token", refreshToken }
            };

            var response = await client.PostAsync($"{_identityServerUrl}/connect/token", new FormUrlEncodedContent(tokenRequest));

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public class GetTokenRequest
        {
            public string SessionId { get; set; } = string.Empty;
        }

        public class LogoutRequest
        {
            public string SessionId { get; set; } = string.Empty;
        }

        
        
        

        private string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Base64UrlEncode(bytes);
        }

        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Base64UrlEncode(challengeBytes);
            }
        }

        private string GenerateRandomString(int length)
        {
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Base64UrlEncode(bytes);
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        
        
        

        private UserInfo ExtractUserInfoFromIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            
            var claims = token.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.First().Value);
            var roles = token.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

            return new UserInfo
            {
                Sub = claims.GetValueOrDefault("sub"),
                Name = claims.GetValueOrDefault("name"),
                Username = claims.GetValueOrDefault("username"),
                Email = claims.GetValueOrDefault("email"),
                EmailVerified = claims.GetValueOrDefault("email_verified") == "true",
                Roles = roles,
                GivenName = claims.GetValueOrDefault("given_name"),
                FamilyName = claims.GetValueOrDefault("family_name")
            };
        }

        
        
        

        
        
        
        
        private async Task<TokenResponse?> ExchangePasswordForTokens(string username, string password)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var tokenRequest = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", CLIENT_ID },
                    { "username", username },
                    { "password", password },
                    { "scope", "openid profile email roles offline_access catalog.read catalog.write cart.manage order.read order.write order.admin discount.read discount.write message.read message.write review.read review.write cargo.read cargo.write image.read image.upload payment.process IdentityServerApi" }
                };

                var requestContent = new FormUrlEncodedContent(tokenRequest);
                var response = await client.PostAsync($"{_identityServerUrl}/connect/token", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return tokenResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        
        
        
        private string GenerateSecureSessionId()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Base64UrlEncode(bytes);
        }

        
        
        
        private string ValidateReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return "/";
            }

            
            if (returnUrl.StartsWith("http://") || returnUrl.StartsWith("https://"))
            {
                var allowedDomains = new[] { "localhost:3000", "mertayaar.com", "www.mertayaar.com" };
                var uri = new Uri(returnUrl);

                if (!allowedDomains.Contains(uri.Authority))
                {
                    return "/";
                }
            }

            return returnUrl;
        }

        
        
        

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? ReturnUrl { get; set; }
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; } = string.Empty;

            [JsonPropertyName("id_token")]
            public string IdToken { get; set; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; } = string.Empty;

            [JsonPropertyName("scope")]
            public string Scope { get; set; } = string.Empty;
        }
    }
}

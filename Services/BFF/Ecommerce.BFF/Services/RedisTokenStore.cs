using Ecommerce.BFF.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecommerce.BFF.Services
{
    
    
    
    
    
    
    public class RedisTokenStore : ITokenStore
    {
        private readonly RedisService _redisService;
        private readonly ILogger<RedisTokenStore> _logger;
        private const string KEY_PREFIX = "bff:session:";
        private static readonly TimeSpan DEFAULT_EXPIRATION = TimeSpan.FromDays(30);

        public RedisTokenStore(RedisService redisService, ILogger<RedisTokenStore> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        public async Task StoreTokensAsync(string sessionId, TokenSet tokens)
        {
            try
            {
                var db = _redisService.GetDb();
                var key = GetKey(sessionId);
                var json = JsonSerializer.Serialize(tokens);
                
                await db.StringSetAsync(key, json, DEFAULT_EXPIRATION);
                _logger.LogInformation("Successfully stored tokens for session {SessionId}", sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store tokens in Redis for session {SessionId}", sessionId);
                
            }
        }

        public async Task<TokenSet> GetTokensAsync(string sessionId)
        {
            try
            {
                var db = _redisService.GetDb();
                var key = GetKey(sessionId);
                var json = await db.StringGetAsync(key);

                if (json.IsNullOrEmpty)
                {
                    _logger.LogWarning("No tokens found for session {SessionId}", sessionId);
                    return null;
                }

                var tokenSet = JsonSerializer.Deserialize<TokenSet>(json);
                _logger.LogInformation("Successfully retrieved tokens for session {SessionId}", sessionId);
                return tokenSet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve tokens from Redis for session {SessionId}", sessionId);
                return null; 
            }
        }

        public async Task RemoveTokensAsync(string sessionId)
        {
            try
            {
                var db = _redisService.GetDb();
                var key = GetKey(sessionId);
                var deleted = await db.KeyDeleteAsync(key);
                
                if (deleted)
                {
                    _logger.LogInformation("Successfully removed tokens for session {SessionId}", sessionId);
                }
                else
                {
                    _logger.LogWarning("No tokens found to remove for session {SessionId}", sessionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove tokens from Redis for session {SessionId}", sessionId);
                
            }
        }

        public async Task UpdateTokensAsync(string sessionId, TokenSet tokens)
        {
            
            await StoreTokensAsync(sessionId, tokens);
        }

        private string GetKey(string sessionId)
        {
            return $"{KEY_PREFIX}{sessionId}";
        }
    }
}

using Ecommerce.Cart.Domain.Dtos;
using Ecommerce.Cart.Infrastructure.Interfaces;
using Ecommerce.Cart.Settings;
using System.Text.Json;

namespace Ecommerce.Cart.Infrastructure.Repositories
{
    
    
    
    
    
    public class RedisCartRepository : ICartRepository
    {
        private readonly RedisService _redisService;
        
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public RedisCartRepository(RedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task<CartTotalDto?> GetCartAsync(string userId)
        {
            var existCart = await _redisService.GetDb().StringGetAsync(userId);
            
            if (string.IsNullOrEmpty(existCart))
            {
                return null;
            }

            var cart = JsonSerializer.Deserialize<CartTotalDto>(existCart!, _jsonOptions);
            
            if (cart != null)
            {
                cart.UserId = userId;
            }
            
            return cart;
        }

        public async Task SaveCartAsync(CartTotalDto cart)
        {
            if (string.IsNullOrEmpty(cart.UserId))
            {
                throw new ArgumentException("UserId is required", nameof(cart));
            }

            var json = JsonSerializer.Serialize(cart, _jsonOptions);
            await _redisService.GetDb().StringSetAsync(cart.UserId, json);
        }

        public async Task DeleteCartAsync(string userId)
        {
            await _redisService.GetDb().KeyDeleteAsync(userId);
        }
    }
}

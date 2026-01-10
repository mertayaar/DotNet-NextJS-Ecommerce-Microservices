using Ecommerce.Cart.Application.Dtos;
using Ecommerce.Cart.Application.Interfaces;
using Ecommerce.Cart.Domain.Dtos;
using Ecommerce.Cart.Infrastructure.Interfaces;

namespace Ecommerce.Cart.Application.Services
{
    
    
    
    
    
    public class CartApplicationService : ICartApplicationService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IDiscountServiceClient _discountClient;
        private readonly ILogger<CartApplicationService> _logger;

        public CartApplicationService(
            ICartRepository cartRepository,
            IDiscountServiceClient discountClient,
            ILogger<CartApplicationService> logger)
        {
            _cartRepository = cartRepository;
            _discountClient = discountClient;
            _logger = logger;
        }

        public async Task<CartTotalDto?> GetCartAsync(string userId)
        {
            return await _cartRepository.GetCartAsync(userId);
        }

        public async Task SaveCartAsync(string userId, CartTotalDto cart)
        {
            cart.UserId = userId;
            await _cartRepository.SaveCartAsync(cart);
        }

        public async Task DeleteCartAsync(string userId)
        {
            await _cartRepository.DeleteCartAsync(userId);
        }

        public async Task<ApplyCouponResult> ApplyCouponAsync(string userId, string couponCode)
        {
            
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return ApplyCouponResult.Failed("Coupon code is required.");
            }

            
            var cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
            {
                return ApplyCouponResult.Failed("Cart is empty. Add items before applying a coupon.");
            }

            
            var coupon = await _discountClient.GetCouponByCodeAsync(couponCode);
            if (coupon == null)
            {
                return ApplyCouponResult.Failed("Invalid coupon code.");
            }

            
            if (!coupon.IsActive)
            {
                return ApplyCouponResult.Failed("This coupon is no longer active.");
            }

            
            if (coupon.ValidDate < DateTime.UtcNow)
            {
                return ApplyCouponResult.Failed("This coupon has expired.");
            }

            
            cart.DiscountCode = coupon.CouponCode;
            cart.DiscountRate = coupon.CouponRate;

            await _cartRepository.SaveCartAsync(cart);

            _logger.LogInformation("Coupon {CouponCode} applied to cart for user {UserId}", couponCode, userId);

            return ApplyCouponResult.Succeeded(coupon.CouponRate);
        }

        public async Task RemoveCouponAsync(string userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            
            if (cart != null)
            {
                cart.DiscountCode = null;
                cart.DiscountRate = 0;
                await _cartRepository.SaveCartAsync(cart);

                _logger.LogInformation("Coupon removed from cart for user {UserId}", userId);
            }
        }
    }
}

using Ecommerce.Cart.Application.Dtos;
using Ecommerce.Cart.Domain.Dtos;

namespace Ecommerce.Cart.Application.Interfaces
{
    
    
    
    
    public interface ICartApplicationService
    {
        
        
        
        Task<CartTotalDto?> GetCartAsync(string userId);

        
        
        
        Task SaveCartAsync(string userId, CartTotalDto cart);

        
        
        
        Task DeleteCartAsync(string userId);

        
        
        
        
        Task<ApplyCouponResult> ApplyCouponAsync(string userId, string couponCode);

        
        
        
        Task RemoveCouponAsync(string userId);
    }
}

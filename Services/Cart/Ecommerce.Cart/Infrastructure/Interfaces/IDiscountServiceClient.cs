using Ecommerce.Cart.Infrastructure.Dtos;

namespace Ecommerce.Cart.Infrastructure.Interfaces
{
    
    
    
    
    public interface IDiscountServiceClient
    {
        Task<CouponDto?> GetCouponByCodeAsync(string couponCode);
    }
}

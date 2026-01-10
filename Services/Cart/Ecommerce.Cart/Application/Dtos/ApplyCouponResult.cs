namespace Ecommerce.Cart.Application.Dtos
{
    
    
    
    
    public class ApplyCouponResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? DiscountRate { get; set; }

        public static ApplyCouponResult Succeeded(int discountRate)
        {
            return new ApplyCouponResult
            {
                Success = true,
                Message = $"Coupon applied! {discountRate}% off.",
                DiscountRate = discountRate
            };
        }

        public static ApplyCouponResult Failed(string message)
        {
            return new ApplyCouponResult
            {
                Success = false,
                Message = message,
                DiscountRate = null
            };
        }
    }
}

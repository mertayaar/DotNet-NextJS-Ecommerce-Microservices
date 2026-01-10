namespace Ecommerce.Cart.Domain.Dtos
{
    public class CartTotalDto
    {
        public string? UserId { get; set; }
        public string? DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new();
        public decimal TotalPrice => CartItems?.Sum(x => x.Price * x.Quantity) ?? 0;
    }
}

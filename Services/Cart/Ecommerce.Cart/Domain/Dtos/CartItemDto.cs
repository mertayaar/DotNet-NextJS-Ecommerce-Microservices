namespace Ecommerce.Cart.Domain.Dtos
{
    public class CartItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; } = string.Empty;
    }
}

namespace Ecommerce.DtoLayer.CartDtos
{
    public class CartItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}

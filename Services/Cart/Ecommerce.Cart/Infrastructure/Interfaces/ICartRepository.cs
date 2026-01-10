namespace Ecommerce.Cart.Infrastructure.Interfaces
{
    using Ecommerce.Cart.Domain.Dtos;

    
    
    
    
    public interface ICartRepository
    {
        Task<CartTotalDto?> GetCartAsync(string userId);
        Task SaveCartAsync(CartTotalDto cart);
        Task DeleteCartAsync(string userId);
    }
}

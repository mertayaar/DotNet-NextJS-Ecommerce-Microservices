
using Ecommerce.DtoLayer.CartDtos;

namespace Ecommerce.WebUI.Services.CartServices
{
    public interface ICartService
    {
        Task<CartTotalDto> GetCart();
        Task SaveCart(CartTotalDto cartTotalDto);
        Task DeleteCart(string userId);
        Task AddToCart(CartItemDto cartItemDto);
        Task<bool> RemoveItemFromCart(string productId);
    }
}

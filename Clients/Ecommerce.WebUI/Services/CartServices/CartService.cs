
using Ecommerce.Common;
using Ecommerce.DtoLayer.CartDtos;

namespace Ecommerce.WebUI.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task AddToCart(CartItemDto cartItemDto)
        {
            var values = await GetCart();

            if (values == null)
                values = new CartTotalDto();

            if (values.CartItems == null)
                values.CartItems = new List<CartItemDto>();

            var existing = values.CartItems.FirstOrDefault(x => x.ProductId == cartItemDto.ProductId);

            if (existing == null)
            {
                values.CartItems.Add(cartItemDto);
            }
            else
            {
                existing.Quantity += cartItemDto.Quantity;
            }
            await SaveCart(values);
        }

        public Task DeleteCart(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CartTotalDto> GetCart()
        {
            var responseMessage = await _httpClient.GetAsync("carts");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<CartTotalDto>>();
            return values.Data;
        }

        public async Task<bool> RemoveItemFromCart(string productId)
        {
            var values = await GetCart();
            var deletedItem = values.CartItems.FirstOrDefault(x => x.ProductId == productId);
            var result = values.CartItems.Remove(deletedItem);
            await SaveCart(values);
            return result;
        }

        public async Task SaveCart(CartTotalDto cartTotalDto)
        {
           await _httpClient.PostAsJsonAsync<CartTotalDto>("carts", cartTotalDto);
        }
    }
}

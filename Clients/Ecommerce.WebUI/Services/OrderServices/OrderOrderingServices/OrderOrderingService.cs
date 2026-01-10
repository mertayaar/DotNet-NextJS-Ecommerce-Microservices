using Ecommerce.Common;
using Ecommerce.DtoLayer.OrderDtos.OrderOrderingDtos;

namespace Ecommerce.WebUI.Services.OrderServices.OrderOrderingServices
{
    public class OrderOrderingService : IOrderOrderingService
    {
        private readonly HttpClient _httpClient;

        public OrderOrderingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ResultOrderingByUserIdDto>> GetOrderingByUserIdAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync($"orderings/GetOrderingByUserId/{id}");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultOrderingByUserIdDto>>>();
            return values.Data;
        }
    }
}

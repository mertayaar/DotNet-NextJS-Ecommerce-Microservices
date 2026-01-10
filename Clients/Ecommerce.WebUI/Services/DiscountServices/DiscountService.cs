using Ecommerce.Common;
using Ecommerce.DtoLayer.DiscountDtos;

namespace Ecommerce.WebUI.Services.DiscountServices
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _httpClient;

        public DiscountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }   

        public async Task<GetDiscountCodeDetailByCode>? GetDiscountCode(string code)
        {
            var responseMessage = await _httpClient.GetAsync($"discounts/GetCodeDetailByCode?code={code}");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<GetDiscountCodeDetailByCode>>();
            return values.Data;
        }
    }
}

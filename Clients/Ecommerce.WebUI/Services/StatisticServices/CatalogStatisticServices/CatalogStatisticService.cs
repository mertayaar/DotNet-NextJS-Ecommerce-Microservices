
using Ecommerce.Common;
using Ecommerce.DtoLayer.CatalogDtos.ProductDetailDtos;

namespace Ecommerce.WebUI.Services.StatisticServices.CatalogStatisticServices
{
    public class CatalogStatisticService : ICatalogStatisticService
    {
        private readonly HttpClient _httpClient;

        public CatalogStatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<long> GetCategoryCount()
        {
            var responseMessage = await _httpClient.GetAsync("statistics/categorycount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<long>>();
            return values.Data;
        }

        public async Task<string> GetMaxPriceProductName()
        {
            var responseMessage = await _httpClient.GetAsync("statistics/MaxPriceProductName");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<string>>();
            return values.Data;
        }

        public async Task<string> GetMinPriceProductName()
        {
            var responseMessage = await _httpClient.GetAsync("statistics/MinPriceProductName");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<string>>();
            return values.Data;
        }

        public async Task<decimal> GetProductAvgPrice()
        {
            var responseMessage = await _httpClient.GetAsync("statistics/ProductAvgPrice");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<decimal>>();
            return values.Data;
        }

        public async Task<long> GetProductCount()
        {
            var responseMessage = await _httpClient.GetAsync("statistics/ProductCount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<long>>();
            return values.Data;
        }

        public async Task<long> GetSponsorBrandCount()
        {
            var responseMessage = await _httpClient.GetAsync("statistics/brandcount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<long>>();
            return values.Data;
        }
    }
}

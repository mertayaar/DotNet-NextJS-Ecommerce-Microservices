
using Ecommerce.Common;
using Ecommerce.DtoLayer.ReviewDtos;

namespace Ecommerce.WebUI.Services.StatisticServices.ReviewStatisticServices
{
    public class ReviewStatisticService : IReviewStatisticService
    {
        private readonly HttpClient _httpClient;

        public ReviewStatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> ActiveReviewCount()
        {
            var responseMessage = await _httpClient.GetAsync("reviews/ActiveReviewCount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return values.Data;
        }

        public async Task<int> PassiveReviewCount()
        {
            var responseMessage = await _httpClient.GetAsync("reviews/PassiveReviewCount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return values.Data;
        }

        public async Task<int> ReviewCount()
        {
            var responseMessage = await _httpClient.GetAsync("reviews/TotalReviewCount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return values.Data;
        }
    }
}

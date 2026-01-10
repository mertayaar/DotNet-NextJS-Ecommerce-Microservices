
using Ecommerce.Common;
using System.Net.Http;

namespace Ecommerce.WebUI.Services.StatisticServices.UserStatisticServices
{
    public class UserStatisticService : IUserStatisticService
    {
        private readonly HttpClient _httpClient;

        public UserStatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> GetUserCount()
        {
            var responseMessage = await _httpClient.GetAsync("api/statistics/usercount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return values.Data;
        }
    }
}

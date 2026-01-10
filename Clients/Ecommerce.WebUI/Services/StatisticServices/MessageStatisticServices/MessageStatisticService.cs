
using Ecommerce.Common;

namespace Ecommerce.WebUI.Services.StatisticServices.MessageStatisticServices
{
    public class MessageStatisticService : IMessageStatisticService
    {
        private readonly HttpClient _httpClient;

        public MessageStatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> GetTotalMessageCount()
        {
            var responseMessage = await _httpClient.GetAsync("usermessage/TotalMessageCount");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return values.Data;
        }
    }
}

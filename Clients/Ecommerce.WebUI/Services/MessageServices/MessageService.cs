using Ecommerce.Common;
using Ecommerce.DtoLayer.CatalogDtos.ProductDtos;
using Ecommerce.DtoLayer.MessageDtos;

namespace Ecommerce.WebUI.Services.MessageServices
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;

        public MessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ResultInboxMessageDto>> GetInboxMessageAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync($"usermessage/GetMessageInbox/{id}");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultInboxMessageDto>>>();
            return values.Data;
        }

        public async Task<List<ResultOutboxMessageDto>> GetOutboxMessageAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync($"usermessage/GetMessageOutbox/{id}");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultOutboxMessageDto>>>();
            return values.Data;
        }

        public async Task<int> GetTotalMessageCountByReceiverId(string id)
        {
            var responseMessage = await _httpClient.GetAsync($"usermessage/TotalMessageCountByReceiverId/{id}");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<int>>();
            return values.Data;
        }
    }
}

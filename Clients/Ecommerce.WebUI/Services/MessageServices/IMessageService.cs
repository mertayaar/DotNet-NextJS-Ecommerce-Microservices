using Ecommerce.DtoLayer.MessageDtos;

namespace Ecommerce.WebUI.Services.MessageServices
{
    public interface IMessageService
    {
        
        Task<List<ResultInboxMessageDto>> GetInboxMessageAsync(string id);
        Task<List<ResultOutboxMessageDto>> GetOutboxMessageAsync(string id);
        Task<int> GetTotalMessageCountByReceiverId(string id);
        
        
        
        
    }
}

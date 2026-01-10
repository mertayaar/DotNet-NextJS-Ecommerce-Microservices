using Ecommerce.DtoLayer.OrderDtos.OrderOrderingDtos;

namespace Ecommerce.WebUI.Services.OrderServices.OrderOrderingServices
{
    public interface IOrderOrderingService
    {
        Task<List<ResultOrderingByUserIdDto>> GetOrderingByUserIdAsync(string id);
    }
}

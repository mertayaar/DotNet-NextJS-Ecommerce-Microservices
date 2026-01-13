using Ecommerce.DtoLayer.OrderDtos.OrderAddressDtos;

namespace Ecommerce.WebUI.Services.OrderServices.OrderAddressServices
{
    public interface IOrderAddressService
    {

        Task CreateOrderAddressAsync(CreateOrderAddressDto createOrderAddressDto);

    }
}

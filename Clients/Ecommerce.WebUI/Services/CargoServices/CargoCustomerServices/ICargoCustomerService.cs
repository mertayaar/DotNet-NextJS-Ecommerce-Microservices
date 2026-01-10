using Ecommerce.DtoLayer.CargoDtos.CargoCustomerDtos;

namespace Ecommerce.WebUI.Services.CargoServices.CargoCustomerServices
{
    public interface ICargoCustomerService
    {
        Task<GetCargoCustomerByIdDto> GetByIdCargoCustomerInfoAsync(string id);

    }
}

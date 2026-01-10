using Ecommerce.DtoLayer.DiscountDtos;

namespace Ecommerce.WebUI.Services.DiscountServices
{
    public interface IDiscountService
    {
        Task<GetDiscountCodeDetailByCode>? GetDiscountCode(string code);
    }
}

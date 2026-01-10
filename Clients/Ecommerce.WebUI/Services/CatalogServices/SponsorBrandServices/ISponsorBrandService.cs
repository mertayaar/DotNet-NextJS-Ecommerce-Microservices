using Ecommerce.DtoLayer.CatalogDtos.SponsorBrandDtos;

namespace Ecommerce.WebUI.Services.CatalogServices.SponsorBrandServices
{
    public interface ISponsorBrandService
    {
        Task<List<ResultSponsorBrandDto>> GetAllSponsorBrandAsync();
        Task CreateSponsorBrandAsync(CreateSponsorBrandDto createSponsorBrandDto);
        Task UpdateSponsorBrandAsync(UpdateSponsorBrandDto updateSponsorBrandDto);
        Task DeleteSponsorBrandAsync(string id);

        Task<UpdateSponsorBrandDto> GetByIdSponsorBrandAsync(string id);
    }
}

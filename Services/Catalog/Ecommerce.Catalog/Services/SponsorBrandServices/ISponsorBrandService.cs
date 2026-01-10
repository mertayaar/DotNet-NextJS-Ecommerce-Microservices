using Ecommerce.Catalog.Dtos.SponsorBrandDtos;

namespace Ecommerce.Catalog.Services.SponsorBrandServices
{
    public interface ISponsorBrandService
    {
        Task<List<ResultSponsorBrandDto>> GetAllSponsorBrandAsync();
        Task CreateSponsorBrandAsync(CreateSponsorBrandDto createSponsorBrandDto);
        Task UpdateSponsorBrandAsync(UpdateSponsorBrandDto updateSponsorBrandDto);
        Task DeleteSponsorBrandAsync(string id);

        Task<GetByIdSponsorBrandDto> GetByIdSponsorBrandAsync(string id);
    }
}

using Ecommerce.Common;
using Ecommerce.DtoLayer.CatalogDtos.SponsorBrandDtos;

namespace Ecommerce.WebUI.Services.CatalogServices.SponsorBrandServices
{
    public class SponsorBrandService : ISponsorBrandService
    {
        private readonly HttpClient _httpClient;

        public SponsorBrandService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateSponsorBrandAsync(CreateSponsorBrandDto createSponsorBrandDto)
        {
            await _httpClient.PostAsJsonAsync<CreateSponsorBrandDto>("sponsorbrands", createSponsorBrandDto);
        }

        public async Task DeleteSponsorBrandAsync(string id)
        {
            await _httpClient.DeleteAsync("sponsorbrands?id=" + id);
        }

        public async Task<List<ResultSponsorBrandDto>> GetAllSponsorBrandAsync()
        {
            var responseMessage = await _httpClient.GetAsync("sponsorbrands");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultSponsorBrandDto>>>();
            return values.Data;
        }

        public async Task<UpdateSponsorBrandDto> GetByIdSponsorBrandAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync("sponsorbrands/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<UpdateSponsorBrandDto>>();
            return values.Data;
        }

        public async Task UpdateSponsorBrandAsync(UpdateSponsorBrandDto updateSponsorBrandDto)
        {
            await _httpClient.PutAsJsonAsync<UpdateSponsorBrandDto>("sponsorbrands", updateSponsorBrandDto);
        }

    }
}

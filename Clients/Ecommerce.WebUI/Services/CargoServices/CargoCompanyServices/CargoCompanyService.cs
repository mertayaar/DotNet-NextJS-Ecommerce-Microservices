using Ecommerce.Common;
using Ecommerce.DtoLayer.CargoDtos.CargoCompanyDtos;
using Ecommerce.DtoLayer.CatalogDtos.AboutDtos;
using System.Net.Http.Json;

namespace Ecommerce.WebUI.Services.CargoServices.CargoCompanyServices
{
    public class CargoCompanyService : ICargoCompanyService
    {
        private readonly HttpClient _httpClient;

        public CargoCompanyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task CreateCargoCompanyAsync(CreateCargoCompanyDto createCargoCompanyDto)
        {
            await _httpClient.PostAsJsonAsync<CreateCargoCompanyDto>("cargocompanies", createCargoCompanyDto);
        }

        public async Task DeleteCargoCompanyAsync(int id)
        {
            await _httpClient.DeleteAsync("cargocompanies?id=" + id);
        }

        public async Task<List<ResultCargoCompanyDto>> GetAllCargoCompanyAsync()
        {
            var responseMessage = await _httpClient.GetAsync("CargoCompanies");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultCargoCompanyDto>>>();
            return values.Data;
        }

        public async Task<UpdateCargoCompanyDto> GetByIdCargoCompanyAsync(int id)
        {
            var responseMessage = await _httpClient.GetAsync("cargocompanies/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<UpdateCargoCompanyDto>>();
            return values.Data;
        }

        public async Task UpdateCargoCompanyAsync(UpdateCargoCompanyDto updateCargoCompanyDto)
        {
            await _httpClient.PutAsJsonAsync<UpdateCargoCompanyDto>("cargocompanies", updateCargoCompanyDto);
        }
    }
}

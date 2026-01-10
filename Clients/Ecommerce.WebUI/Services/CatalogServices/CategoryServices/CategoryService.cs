using Ecommerce.Common;
using Ecommerce.DtoLayer.CatalogDtos.CategoryDtos;

namespace Ecommerce.WebUI.Services.CatalogServices.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            await  _httpClient.PostAsJsonAsync<CreateCategoryDto>("categories", createCategoryDto);
        }

        public async Task DeleteCategoryAsync(string id)
        {
           await  _httpClient.DeleteAsync("categories?id="+id);
        }

        public async Task<List<ResultCategoryDto>> GetAllCategoryAsync()
        {
          var responseMessage =  await _httpClient.GetAsync("categories");
          var apiResponse = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultCategoryDto>>>();
          return apiResponse?.Data ?? new List<ResultCategoryDto>();
        }

        public async Task<UpdateCategoryDto> GetByIdCategoryAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync("categories/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<UpdateCategoryDto>>();
            return values.Data;
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            await _httpClient.PutAsJsonAsync<UpdateCategoryDto>("categories", updateCategoryDto);
        }
    }
}

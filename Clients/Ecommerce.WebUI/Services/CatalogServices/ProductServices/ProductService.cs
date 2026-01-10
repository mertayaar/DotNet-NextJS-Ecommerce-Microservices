using Ecommerce.Common;
using Ecommerce.DtoLayer.CatalogDtos.ProductDtos;
using Newtonsoft.Json;

namespace Ecommerce.WebUI.Services.CatalogServices.ProductServices
{
    public class ProductService : IProductService
    {

        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var response = await _httpClient.PostAsJsonAsync<CreateProductDto>("products", createProductDto);
            if (!response.IsSuccessStatusCode)
            {
               var content = await response.Content.ReadAsStringAsync();
               Console.WriteLine($"[ProductService] CreateProductAsync failed: {response.StatusCode} - {content}");
               
            }
        }

        public async Task DeleteProductAsync(string id)
        {
            await _httpClient.DeleteAsync("products?id=" + id);
        }

        public async Task<List<ResultProductDto>> GetAllProductAsync()
        {
            var responseMessage = await _httpClient.GetAsync("products");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultProductDto>>>();
            return values.Data;
        }

        public async Task<UpdateProductDto> GetByIdProductAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync("products/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<UpdateProductDto>>();
            return values.Data;
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryAsync()
        {
            var responseMessage = await _httpClient.GetAsync("products/ProductListWithCategory");
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<ApiResponse<List<ResultProductsWithCategoryDto>>>(jsonData);
            return values.Data;
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryByCategoryIdAsync(string CategoryId)
        {
            var responseMessage = await _httpClient.GetAsync("products/ProductListWithCategoryByCategoryId/" + CategoryId);
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<ApiResponse<List<ResultProductsWithCategoryDto>>>(jsonData);
            return values.Data;
        }


        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            await _httpClient.PutAsJsonAsync<UpdateProductDto>("products", updateProductDto);

        }
    }
}

using Ecommerce.Common;
using Ecommerce.DtoLayer.ReviewDtos;

namespace Ecommerce.WebUI.Services.ReviewServices
{
    public class ReviewService : IReviewService
    {
        private readonly HttpClient _httpClient;

        public ReviewService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            await _httpClient.PostAsJsonAsync<CreateReviewDto>("reviews", createReviewDto);
        }

        public async Task DeleteReviewAsync(string id)
        {
            await _httpClient.DeleteAsync("reviews?id=" + id);
        }

        public async Task<List<ResultReviewDto>> GetAllReviewAsync()
        {
            var responseMessage = await _httpClient.GetAsync("reviews");
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultReviewDto>>>();
            return values.Data;
        }

        public async Task<UpdateReviewDto> GetByIdReviewAsync(string id)
        {
            var responseMessage = await _httpClient.GetAsync("reviews/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<UpdateReviewDto>>();
            return values.Data;
        }

        public async Task<List<ResultReviewDto>> ReviewListByProductId(string id)
        {
            var responseMessage = await _httpClient.GetAsync("reviews/ReviewListByProductId/" + id);
            var values = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<ResultReviewDto>>>();
            return values.Data;
        }

        public async Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            await _httpClient.PutAsJsonAsync<UpdateReviewDto>("reviews", updateReviewDto);
        }
    }
}

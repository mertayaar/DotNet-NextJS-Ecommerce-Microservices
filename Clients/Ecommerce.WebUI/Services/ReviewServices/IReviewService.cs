using Ecommerce.DtoLayer.ReviewDtos;

namespace Ecommerce.WebUI.Services.ReviewServices
{
    public interface IReviewService
    {
        Task<List<ResultReviewDto>> GetAllReviewAsync();
        Task<List<ResultReviewDto>> ReviewListByProductId(string id);
        Task CreateReviewAsync(CreateReviewDto createReviewDto);
        Task UpdateReviewAsync(UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(string id);

        Task<UpdateReviewDto> GetByIdReviewAsync(string id);
    }
}

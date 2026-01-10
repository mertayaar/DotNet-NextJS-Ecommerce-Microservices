namespace Ecommerce.WebUI.Services.StatisticServices.ReviewStatisticServices
{
    public interface IReviewStatisticService
    {
        Task<int> ReviewCount();
        Task<int> ActiveReviewCount();
        Task<int> PassiveReviewCount();
    }
}

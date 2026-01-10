namespace Ecommerce.Catalog.Services.StatisticServices
{
    public interface IStatisticService
    {
        Task<long> GetCategoryCount();
        Task<long> GetProductCount();
        
        Task<long> GetSponsorBrandCount();
        Task<decimal> GetProductAvgPrice();
        Task<string> GetMaxPriceProductName();
        Task<string> GetMinPriceProductName();



    }
}

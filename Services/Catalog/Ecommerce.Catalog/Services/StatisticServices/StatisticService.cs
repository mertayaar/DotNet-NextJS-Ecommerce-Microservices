using Ecommerce.Catalog.Entities;
using Ecommerce.Catalog.Settings;
using MongoDB.Driver;

namespace Ecommerce.Catalog.Services.StatisticServices
{
    public class StatisticService : IStatisticService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<SponsorBrand> _brandCollection;

        public StatisticService(IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _brandCollection = database.GetCollection<SponsorBrand>(databaseSettings.SponsorBrandCollectionName);
        }

        public async Task<long> GetProductCount()
        {
            return _productCollection.CountDocuments(FilterDefinition<Product>.Empty);
        }

        public async Task<decimal> GetProductAvgPrice()
        {
            var products = _productCollection.Find(FilterDefinition<Product>.Empty).ToList();

            if (products.Count == 0)
                return 0;

            return products.Average(p => p.ProductPrice);
        }

        public async Task<long> GetCategoryCount()
        {
            return _categoryCollection.CountDocuments(FilterDefinition<Category>.Empty);
        }

        public async Task<long> GetSponsorBrandCount()
        {
            return _brandCollection.CountDocuments(FilterDefinition<SponsorBrand>.Empty);
        }

        public async Task<string> GetMaxPriceProductName()
        {
            var sort = Builders<Product>.Sort.Descending(p => p.ProductPrice);
            var product = await _productCollection.Find(FilterDefinition<Product>.Empty)
                                                  .Sort(sort)
                                                  .Limit(1)
                                                  .FirstOrDefaultAsync();

            return product?.ProductName ?? "";
        }

        public async Task<string> GetMinPriceProductName()
        {
            var sort = Builders<Product>.Sort.Ascending(p => p.ProductPrice);
            var product = await _productCollection.Find(FilterDefinition<Product>.Empty)
                                                  .Sort(sort)
                                                  .Limit(1)
                                                  .FirstOrDefaultAsync();

            return product?.ProductName ?? "";
        }
    }
}

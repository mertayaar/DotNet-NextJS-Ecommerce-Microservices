using AutoMapper;
using Ecommerce.Catalog.Dtos.ProductDtos;
using Ecommerce.Catalog.Entities;
using Ecommerce.Catalog.Helpers;
using Ecommerce.Catalog.Settings;
using MongoDB.Driver;

namespace Ecommerce.Catalog.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Category> _categoryCollection;


        public ProductService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _productCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
            _categoryCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }


        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var values = _mapper.Map<Product>(createProductDto);
            
            
            if (string.IsNullOrWhiteSpace(values.ProductSlug))
            {
                var baseSlug = SlugHelper.GenerateSlug(createProductDto.ProductName);
                var existingSlugs = await _productCollection
                    .Find(x => x.ProductSlug != null && x.ProductSlug.StartsWith(baseSlug))
                    .Project(x => x.ProductSlug)
                    .ToListAsync();
                values.ProductSlug = SlugHelper.GenerateUniqueSlug(baseSlug, existingSlugs);
            }
            
            await _productCollection.InsertOneAsync(values);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _productCollection.DeleteOneAsync(x => x.ProductId == id);
        }

        public async Task<List<ResultProductDto>> GetAllProductAsync()
        {
            var values = await _productCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultProductDto>>(values);
        }

        public async Task<GetByIdProductDto> GetByIdProductAsync(string id)
        {
            var values = await _productCollection.Find<Product>(x => x.ProductId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdProductDto>(values);
        }

        public async Task<GetByIdProductDto> GetBySlugProductAsync(string slug)
        {
            var values = await _productCollection.Find<Product>(x => x.ProductSlug == slug).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdProductDto>(values);
        }

        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var values = _mapper.Map<Product>(updateProductDto);
            
            
            if (string.IsNullOrWhiteSpace(values.ProductSlug))
            {
                var baseSlug = SlugHelper.GenerateSlug(updateProductDto.ProductName);
                var existingSlugs = await _productCollection
                    .Find(x => x.ProductSlug != null && x.ProductSlug.StartsWith(baseSlug) && x.ProductId != updateProductDto.ProductId)
                    .Project(x => x.ProductSlug)
                    .ToListAsync();
                values.ProductSlug = SlugHelper.GenerateUniqueSlug(baseSlug, existingSlugs);
            }
            
            await _productCollection.FindOneAndReplaceAsync(x => x.ProductId == updateProductDto.ProductId, values);
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryAsync()
        {
            var values = await _productCollection.Find(x => true).ToListAsync();
            foreach (var item in values)
            {
                item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstAsync();

            }
            return _mapper.Map<List<ResultProductsWithCategoryDto>>(values);
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryByCategoryIdAsync(string CategoryId)
        {
            var values = await _productCollection.Find(x => x.CategoryId == CategoryId).ToListAsync();

            foreach (var item in values)
            {
                item.Category = await _categoryCollection.Find<Category>(x => x.CategoryId == item.CategoryId).FirstAsync();
            }

            return _mapper.Map<List<ResultProductsWithCategoryDto>>(values);
        }
    }
}

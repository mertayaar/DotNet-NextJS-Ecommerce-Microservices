using AutoMapper;
using Ecommerce.Catalog.Dtos.SponsorBrandDtos;
using Ecommerce.Catalog.Entities;
using Ecommerce.Catalog.Settings;
using MongoDB.Driver;

namespace Ecommerce.Catalog.Services.SponsorBrandServices
{
    public class SponsorBrandService : ISponsorBrandService
    {
        private readonly IMongoCollection<SponsorBrand> _sponsorBrandCollection;
        private readonly IMapper _mapper;

        public SponsorBrandService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _sponsorBrandCollection = database.GetCollection<SponsorBrand>(_databaseSettings.SponsorBrandCollectionName);
            _mapper = mapper;
        }
        public async Task CreateSponsorBrandAsync(CreateSponsorBrandDto createSponsorBrandDto)
        {
            var value = _mapper.Map<SponsorBrand>(createSponsorBrandDto);
            await _sponsorBrandCollection.InsertOneAsync(value);
        }

        public async Task DeleteSponsorBrandAsync(string id)
        {
            await _sponsorBrandCollection.DeleteOneAsync(x => x.SponsorBrandId == id);
        }

        public async Task<List<ResultSponsorBrandDto>> GetAllSponsorBrandAsync()
        {
            var values = await _sponsorBrandCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultSponsorBrandDto>>(values);
        }

        public async Task<GetByIdSponsorBrandDto> GetByIdSponsorBrandAsync(string id)
        {
            var values = await _sponsorBrandCollection.Find<SponsorBrand>(x => x.SponsorBrandId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetByIdSponsorBrandDto>(values);
        }

        public async Task UpdateSponsorBrandAsync(UpdateSponsorBrandDto updateSponsorBrandDto)
        {
            var values = _mapper.Map<SponsorBrand>(updateSponsorBrandDto);
            await _sponsorBrandCollection.FindOneAndReplaceAsync(x => x.SponsorBrandId == updateSponsorBrandDto.SponsorBrandId, values);
        }
    }
}

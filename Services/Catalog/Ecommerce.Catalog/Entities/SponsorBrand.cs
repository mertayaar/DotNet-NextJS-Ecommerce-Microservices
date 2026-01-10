using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Catalog.Entities
{
    public class SponsorBrand
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SponsorBrandId { get; set; }
        public string BrandName { get; set; }
        public string ImageUrl { get; set; }
    }
}

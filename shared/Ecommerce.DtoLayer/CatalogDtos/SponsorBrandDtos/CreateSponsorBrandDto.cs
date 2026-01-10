using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DtoLayer.CatalogDtos.SponsorBrandDtos
{
    public class CreateSponsorBrandDto
    {
        public string SponsorBrandId { get; set; }
        public string BrandName { get; set; }
        public string ImageUrl { get; set; }
    }
}

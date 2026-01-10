using Ecommerce.Catalog.Services.StatisticServices;
using Ecommerce.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecommerce.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }
        [HttpGet("BrandCount")]
        public async Task<IActionResult> GetBrandCount()
        {
            var brandCount = await _statisticService.GetSponsorBrandCount();
            return Ok(ApiResponse<long>.Ok(brandCount));
        }

        [HttpGet("CategoryCount")]
        public async Task<IActionResult> GetCategoryCount()
        {
            var categoryCount = await _statisticService.GetCategoryCount();
            return Ok(ApiResponse<long>.Ok(categoryCount));
        }

        [HttpGet("ProductCount")]
        public async Task<IActionResult> GetProductCountAsync()
        {
            var productCount = await _statisticService.GetProductCount();
            return Ok(ApiResponse<long>.Ok(productCount));
        }
        [HttpGet("ProductAvgPrice")]
        public async Task<IActionResult> GetProductAvgPriceAsync()
        {
            var productPriceAvg = await _statisticService.GetProductAvgPrice();
            return Ok(ApiResponse<decimal>.Ok(productPriceAvg));
        }
        [HttpGet("MaxPriceProductName")]
        public async Task<IActionResult> GetMaxPriceProductName()
        {
            var productName = await _statisticService.GetMaxPriceProductName();
            return Ok(ApiResponse<string>.Ok(productName));
        }
        [HttpGet("MinPriceProductName")]
        public async Task<IActionResult> GetMinPriceProductName()
        {
            var productName = await _statisticService.GetMinPriceProductName();
            return Ok(ApiResponse<string>.Ok(productName));
        }
    }
}

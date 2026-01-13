using Microsoft.AspNetCore.Authorization;
using Ecommerce.WebUI.Services.StatisticServices.CatalogStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.DiscountStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.MessageStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.ReviewStatisticServices;
using Ecommerce.WebUI.Services.StatisticServices.UserStatisticServices;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/Statistics")]

    public class StatisticsController : Controller
    {
        private readonly ICatalogStatisticService _catalogStatisticService;
        private readonly IUserStatisticService _userStatisticService;
        private readonly IReviewStatisticService _reviewStatisticService;
        private readonly IDiscountStatisticService _discountStatisticService;
        private readonly IMessageStatisticService _messageStatisticService;

        public StatisticsController(ICatalogStatisticService catalogStatisticService, IUserStatisticService userStatisticService, IReviewStatisticService reviewStatisticService, IDiscountStatisticService discountStatisticService, IMessageStatisticService messageStatisticService)
        {
            _catalogStatisticService = catalogStatisticService;
            _userStatisticService = userStatisticService;
            _reviewStatisticService = reviewStatisticService;
            _discountStatisticService = discountStatisticService;
            _messageStatisticService = messageStatisticService;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var brandCount = await _catalogStatisticService.GetSponsorBrandCount();
            var productCount = await _catalogStatisticService.GetProductCount();
            var categoryCount = await _catalogStatisticService.GetCategoryCount();
            var maxPriceProductName = await _catalogStatisticService.GetMaxPriceProductName();
            var minPriceProductName = await _catalogStatisticService.GetMinPriceProductName();
            var productAvgPrice = await _catalogStatisticService.GetProductAvgPrice();
            var userCount = await _userStatisticService.GetUserCount();
            var reviewCount = await _reviewStatisticService.ReviewCount();
            var activeReviewCount = await _reviewStatisticService.ActiveReviewCount();
            var passiveReviewCount = await _reviewStatisticService.PassiveReviewCount();
            var discountCouponCount = await _discountStatisticService.GetDiscountCouponCount();
            var messageCount = await _messageStatisticService.GetTotalMessageCount();
            ViewBag.brandCount = brandCount;
            ViewBag.productCount = productCount;
            ViewBag.categoryCount = categoryCount;
            ViewBag.maxPriceProductName = maxPriceProductName;
            ViewBag.minPriceProductName = minPriceProductName;
            ViewBag.productAvgPrice = productAvgPrice.ToString("N2");
            ViewBag.userCount = userCount;
            ViewBag.reviewCount = reviewCount;
            ViewBag.activeReviewCount = activeReviewCount;
            ViewBag.passiveReviewCount = passiveReviewCount;
            ViewBag.discountCouponCount = discountCouponCount;
            ViewBag.messageCount = messageCount;
            return View();
        }
    }
}

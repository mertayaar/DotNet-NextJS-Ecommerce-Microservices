using Ecommerce.WebUI.Services.Interfaces;
using Ecommerce.WebUI.Services.MessageServices;
using Ecommerce.WebUI.Services.ReviewServices;
using Ecommerce.WebUI.Services.StatisticServices.ReviewStatisticServices;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.WebUI.Areas.Admin.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminLayoutHeaderComponentPartial : ViewComponent
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IReviewStatisticService _reviewStatisticService;


        public _AdminLayoutHeaderComponentPartial(IMessageService messageService, IUserService userService, IReviewStatisticService reviewStatisticService)
        {
            _messageService = messageService;
            _userService = userService;
            _reviewStatisticService = reviewStatisticService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userService.GetUserInfo();
            var messageCount = await _messageService.GetTotalMessageCountByReceiverId(user.Id);
            ViewBag.messageCount = messageCount;

            var reviewCount = await _reviewStatisticService.ReviewCount();

            ViewBag.reviewCount = reviewCount;
            return View();
        }
    }
}

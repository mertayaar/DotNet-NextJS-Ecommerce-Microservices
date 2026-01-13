using Microsoft.AspNetCore.Authorization;
using Ecommerce.WebUI.Services.CargoServices.CargoCustomerServices;
using Ecommerce.WebUI.Services.UserIdentityServices;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly ICargoCustomerService _cargoCustomerService;

        public UserController(IUserIdentityService userIdentityService, ICargoCustomerService cargoCustomerService)
        {
            _userIdentityService = userIdentityService;
            _cargoCustomerService = cargoCustomerService;
        }

        public async Task<IActionResult> UserList()
        {
            var values = await _userIdentityService.GetAllUserListAsync();
            return View(values);
        }
        public async Task<IActionResult> UserAddressInfo(string id)
        {
            var values = await _cargoCustomerService.GetByIdCargoCustomerInfoAsync(id);
            return View(values);
        }
    }
}

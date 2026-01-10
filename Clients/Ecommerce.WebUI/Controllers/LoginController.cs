using Ecommerce.DtoLayer.IdentityDtos.LoginDtos;
using Ecommerce.WebUI.Models;
using Ecommerce.WebUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecommerce.WebUI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IIdentityService _identityService;



        public LoginController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInDto signInDto)
        {
            var result = await _identityService.SignIn(signInDto);
            if (result)
            {
                return RedirectToAction("Index", "Statistics", new { area = "Admin" });
            }
            
            ModelState.AddModelError("", "Invalid username or password.");
            return View(signInDto);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}

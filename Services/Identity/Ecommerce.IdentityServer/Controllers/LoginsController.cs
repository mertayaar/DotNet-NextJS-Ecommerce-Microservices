using Ecommerce.IdentityServer.Dtos;
using Ecommerce.IdentityServer.Models;
using Ecommerce.IdentityServer.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecommerce.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public LoginsController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginDto userLoginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(userLoginDto.Username, userLoginDto.Password, false, false);
            var user = await _userManager.FindByNameAsync(userLoginDto.Username);
            if (result.Succeeded)
            {
                GetCheckAppUserViewModel userViewModel = new GetCheckAppUserViewModel();
                userViewModel.Username = userLoginDto.Username;
                userViewModel.Id = user.Id;
                var token = JwtTokenGenerator.GenerateToken(userViewModel);

                return Ok(token);
              
            }
            return Unauthorized(new { Message = "Invalid username or password" });
        }
    }
}

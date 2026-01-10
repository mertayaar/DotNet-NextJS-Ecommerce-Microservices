using Ecommerce.Common;
using Ecommerce.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Ecommerce.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public StatisticsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("UserCount")]
        public async Task<IActionResult> GetUserCount()
        {
            var userCount = _userManager.Users.Count();
            return Ok(ApiResponse<int>.Ok(userCount));
        }
    }
}

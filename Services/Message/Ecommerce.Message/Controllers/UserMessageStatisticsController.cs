using Ecommerce.Message.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Message.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserMessageStatisticsController : ControllerBase
    {
        private readonly IUserMessageService _userMessageService;

        public UserMessageStatisticsController(IUserMessageService userMessageService)
        {
            _userMessageService = userMessageService;
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalMessageCount()
        {
            var values = await _userMessageService.GetTotalMessageCount();
            return Ok(values);
        }

    }
}

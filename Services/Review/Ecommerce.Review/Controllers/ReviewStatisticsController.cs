using Ecommerce.Review.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Review.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ReviewStatisticsController : ControllerBase
    {
        private readonly ReviewContext _context;

        public ReviewStatisticsController(ReviewContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewCount()
        {
            var values = await _context.UserReviews.CountAsync();
            return Ok(values);
        }
    }
}

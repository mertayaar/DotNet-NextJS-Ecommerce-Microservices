using Ecommerce.Review.Context;
using Ecommerce.Review.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Common;

namespace Ecommerce.Review.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewContext _context;

        public ReviewsController(ReviewContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ReviewList()
        {
            var values = await _context.UserReviews.ToListAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<UserReview>>.Ok(values));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(UserReview userReview)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _context.UserReviews.AddAsync(userReview);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var value = await _context.UserReviews.FindAsync(id);
            if (value == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

            _context.UserReviews.Remove(value);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("ReviewListByProductId/{id}")]
        public async Task<IActionResult> ReviewListByProductId(string id)
        {
            var values = await _context.UserReviews.Where(x => x.ProductId == id).ToListAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<UserReview>>.Ok(values));
        }


        [HttpPut]
        public async Task<IActionResult> UpdateReview(UserReview userReview)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            _context.UserReviews.Update(userReview);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            var value = await _context.UserReviews.FindAsync(id);
            if (value == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

            return Ok(ApiResponse<UserReview>.Ok(value));
        }

        [HttpGet("ActiveReviewCount")]
        public async Task<IActionResult> ActiveReviewCount()
        {
            var count = await _context.UserReviews.CountAsync(x => x.Status == true);
            return Ok(ApiResponse<int>.Ok(count));
        }
        [HttpGet("PassiveReviewCount")]
        public async Task<IActionResult> PassiveReviewCount()
        {
            var count = await _context.UserReviews.CountAsync(x => x.Status == false);
            return Ok(ApiResponse<int>.Ok(count));
        }
        [HttpGet("TotalReviewCount")]
        public async Task<IActionResult> TotalReviewCount()
        {
            var count = await _context.UserReviews.CountAsync();
            return Ok(ApiResponse<int>.Ok(count));
        }
    }
}


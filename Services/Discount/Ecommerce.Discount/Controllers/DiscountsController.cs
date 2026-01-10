using Ecommerce.Discount.Dtos;
using Ecommerce.Discount.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Discount.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> DiscountCouponList()
        {
            var values = await _discountService.GetAllDiscountCouponAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultDiscountCouponDto>>.Ok(values));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountCouponById(int id)
        {
            var values = await _discountService.GetByIdDiscountCouponAsync(id);
            if (values == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

            return Ok(ApiResponse<GetByIdDiscountCouponDto>.Ok(values));
        }
        [HttpGet("GetCodeDetailByCode")]
        public async Task<IActionResult> GetCodeDetailByCode([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(ApiResponse.Fail(ApiMessages.CodeRequired));

            var values = await _discountService.GetCodeDetailByCodeAsync(code);
            if (values == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.CodeNotFound, code)));

            return Ok(ApiResponse<ResultDiscountCouponDto>.Ok(values));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscountCoupon([FromBody] CreateDiscountCouponDto createCouponDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _discountService.CreateDiscountCouponAsync(createCouponDto);

            if (!string.IsNullOrWhiteSpace(createCouponDto.CouponCode))
            {
                return CreatedAtAction(nameof(GetCodeDetailByCode), new { code = createCouponDto.CouponCode }, ApiResponse.Ok(ApiMessages.Created));
            }

            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscountCoupon(int id)
        {
            await _discountService.DeleteDiscountCouponAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDiscountCoupon([FromBody] UpdateDiscountCouponDto updateCouponDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _discountService.UpdateDiscountCouponAsync(updateCouponDto);
            return NoContent();
        }

        [HttpGet("DiscountCouponCount")]
        public async Task<IActionResult> GetDiscountCouponCount()
        {
            var count = await _discountService.GetDiscountCouponCount();
            
            return Ok(ApiResponse<int>.Ok(count));
        }
    }
}


using Ecommerce.Cart.Application.Interfaces;
using Ecommerce.Cart.Domain.Dtos;
using Ecommerce.Cart.Settings;
using Ecommerce.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Cart.Controllers
{
    
    
    
    
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartApplicationService _cartService;
        private readonly ILoginService _loginService;

        public CartsController(ICartApplicationService cartService, ILoginService loginService)
        {
            _cartService = cartService;
            _loginService = loginService;
        }

        
        
        
        [HttpGet]
        public async Task<IActionResult> GetMyCartDetail()
        {
            var cart = await _cartService.GetCartAsync(_loginService.GetUserId);
            
            if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
            {
                return NoContent();
            }

            return Ok(ApiResponse<object>.Ok(cart));
        }

        
        
        
        [HttpPost]
        public async Task<IActionResult> SaveCart([FromBody] CartTotalDto cartTotalDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _cartService.SaveCartAsync(_loginService.GetUserId, cartTotalDto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
        }

        
        
        
        [HttpDelete]
        public async Task<IActionResult> DeleteCart()
        {
            await _cartService.DeleteCartAsync(_loginService.GetUserId);
            return NoContent();
        }

        
        
        
        [HttpPost("apply-coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CouponCode))
            {
                return BadRequest(ApiResponse.Fail("Coupon code is required."));
            }

            var result = await _cartService.ApplyCouponAsync(_loginService.GetUserId, request.CouponCode);

            if (!result.Success)
            {
                return BadRequest(ApiResponse.Fail(result.Message));
            }

            return Ok(ApiResponse<object>.Ok(new 
            { 
                message = result.Message, 
                discountRate = result.DiscountRate 
            }));
        }

        
        
        
        [HttpDelete("remove-coupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            await _cartService.RemoveCouponAsync(_loginService.GetUserId);
            return Ok(ApiResponse.Ok("Coupon removed successfully."));
        }
    }

    
    
    
    public class ApplyCouponRequest
    {
        public string CouponCode { get; set; } = string.Empty;
    }
}

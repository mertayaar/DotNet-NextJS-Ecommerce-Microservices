using Ecommerce.Catalog.Dtos.OfferDiscountDtos;
using Ecommerce.Catalog.Services.OfferDiscountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Ecommerce.Common;

namespace Ecommerce.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OfferDiscountsController : ControllerBase
    {
        private readonly IOfferDiscountService _offerDiscountService;
        private readonly ILogger<OfferDiscountsController> _logger;

        public OfferDiscountsController(IOfferDiscountService offerDiscountService, ILogger<OfferDiscountsController> logger)
        {
            _offerDiscountService = offerDiscountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> OfferDiscountList()
        {
            try
            {
                var values = await _offerDiscountService.GetAllOfferDiscountAsync();
                if (values == null || values.Count == 0)
                    return NoContent();

                return Ok(ApiResponse<List<ResultOfferDiscountDto>>.Ok(values));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetListError);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveError));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferDiscountById(string id)
        {
            try
            {
                var value = await _offerDiscountService.GetByIdOfferDiscountAsync(id);
                if (value == null)
                    return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

                return Ok(ApiResponse<GetByIdOfferDiscountDto>.Ok(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetByIdError, id);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveByIdError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOfferDiscount(CreateOfferDiscountDto createOfferDiscountDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _offerDiscountService.CreateOfferDiscountAsync(createOfferDiscountDto);
                return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, ApiLogMessages.ValidationFailed, aex.Message);
                return BadRequest(ApiResponse.Fail(aex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.CreateError);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.CreateError));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOfferDiscount(string id)
        {
            try
            {
                await _offerDiscountService.DeleteOfferDiscountAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.DeleteError, id);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.DeleteError));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOfferDiscount(UpdateOfferDiscountDto updateOfferDiscountDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _offerDiscountService.UpdateOfferDiscountAsync(updateOfferDiscountDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, updateOfferDiscountDto.OfferDiscountId)));
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, ApiLogMessages.ValidationFailed, aex.Message);
                return BadRequest(ApiResponse.Fail(aex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.UpdateError, updateOfferDiscountDto.OfferDiscountId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.UpdateError));
            }
        }
    }
}


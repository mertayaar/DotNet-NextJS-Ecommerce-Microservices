using Ecommerce.Catalog.Dtos.SpecialOfferDtos;
using Ecommerce.Catalog.Services.SpecialOfferServices;
using Ecommerce.Catalog.Services.SpecialOfferServices;
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
    public class SpecialOffersController : ControllerBase
    {
        private readonly ISpecialOfferService _specialOfferService;
        private readonly ILogger<SpecialOffersController> _logger;

        public SpecialOffersController(ISpecialOfferService specialOfferService, ILogger<SpecialOffersController> logger)
        {
            _specialOfferService = specialOfferService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SpecialOfferList()
        {
            try
            {
                var values = await _specialOfferService.GetAllSpecialOfferAsync();
                if (values == null || values.Count == 0)
                    return NoContent();

                return Ok(ApiResponse<List<ResultSpecialOfferDto>>.Ok(values));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetListError);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveError));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecialOfferById(string id)
        {
            try
            {
                var value = await _specialOfferService.GetByIdSpecialOfferAsync(id);
                if (value == null)
                    return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

                return Ok(ApiResponse<GetByIdSpecialOfferDto>.Ok(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetByIdError, id);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveByIdError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpecialOffer(CreateSpecialOfferDto createSpecialOfferDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _specialOfferService.CreateSpecialOfferAsync(createSpecialOfferDto);
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
        public async Task<IActionResult> DeleteSpecialOffer(string id)
        {
            try
            {
                await _specialOfferService.DeleteSpecialOfferAsync(id);
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
        public async Task<IActionResult> UpdateSpecialOffer(UpdateSpecialOfferDto updateSpecialOfferDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _specialOfferService.UpdateSpecialOfferAsync(updateSpecialOfferDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, updateSpecialOfferDto.SpecialOfferId)));
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, ApiLogMessages.ValidationFailed, aex.Message);
                return BadRequest(ApiResponse.Fail(aex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.UpdateError, updateSpecialOfferDto.SpecialOfferId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.UpdateError));
            }
        }
    }
}


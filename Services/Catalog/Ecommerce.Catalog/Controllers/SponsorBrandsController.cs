using Ecommerce.Catalog.Dtos.SponsorBrandDtos;
using Ecommerce.Catalog.Services.SponsorBrandServices;
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
    public class SponsorBrandsController : ControllerBase
    {
        private readonly ISponsorBrandService _sponsorBrandService;
        private readonly ILogger<SponsorBrandsController> _logger;

        public SponsorBrandsController(ISponsorBrandService sponsorBrandService, ILogger<SponsorBrandsController> logger)
        {
            _sponsorBrandService = sponsorBrandService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> SponsorBrandList()
        {
            try
            {
                var values = await _sponsorBrandService.GetAllSponsorBrandAsync();
                if (values == null || values.Count == 0)
                    return NoContent();

                return Ok(ApiResponse<List<ResultSponsorBrandDto>>.Ok(values));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetListError);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveError));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSponsorBrandById(string id)
        {
            try
            {
                var value = await _sponsorBrandService.GetByIdSponsorBrandAsync(id);
                if (value == null)
                    return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

                return Ok(ApiResponse<GetByIdSponsorBrandDto>.Ok(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetByIdError, id);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveByIdError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSponsorBrand(CreateSponsorBrandDto createSponsorBrandDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _sponsorBrandService.CreateSponsorBrandAsync(createSponsorBrandDto);
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
        public async Task<IActionResult> DeleteSponsorBrand(string id)
        {
            try
            {
                await _sponsorBrandService.DeleteSponsorBrandAsync(id);
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
        public async Task<IActionResult> UpdateSponsorBrand(UpdateSponsorBrandDto updateSponsorBrandDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _sponsorBrandService.UpdateSponsorBrandAsync(updateSponsorBrandDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, updateSponsorBrandDto.SponsorBrandId)));
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, ApiLogMessages.ValidationFailed, aex.Message);
                return BadRequest(ApiResponse.Fail(aex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.UpdateError, updateSponsorBrandDto.SponsorBrandId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.UpdateError));
            }
        }
    }
}


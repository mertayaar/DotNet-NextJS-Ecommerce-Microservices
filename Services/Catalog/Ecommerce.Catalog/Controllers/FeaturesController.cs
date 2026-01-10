using Ecommerce.Catalog.Dtos.FeatureDtos;
using Ecommerce.Catalog.Services.FeatureServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Common;

namespace Ecommerce.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureService _featureService;
        private readonly ILogger<FeaturesController> _logger;

        public FeaturesController(IFeatureService featureService, ILogger<FeaturesController> logger)
        {
            _featureService = featureService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> FeatureList()
        {
            try
            {
                var values = await _featureService.GetAllFeatureAsync();
                if (values == null || values.Count == 0)
                    return NoContent();

                return Ok(ApiResponse<List<ResultFeatureDto>>.Ok(values));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetListError);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveError));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeatureById(string id)
        {
            try
            {
                var value = await _featureService.GetByIdFeatureAsync(id);
                if (value == null)
                    return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

                return Ok(ApiResponse<GetByIdFeatureDto>.Ok(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.GetByIdError, id);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.RetrieveByIdError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeature(CreateFeatureDto createFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _featureService.CreateFeatureAsync(createFeatureDto);
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
        public async Task<IActionResult> DeleteFeature(string id)
        {
            try
            {
                await _featureService.DeleteFeatureAsync(id);
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
        public async Task<IActionResult> UpdateFeature(UpdateFeatureDto updateFeatureDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            try
            {
                await _featureService.UpdateFeatureAsync(updateFeatureDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, updateFeatureDto.FeatureId)));
            }
            catch (ArgumentException aex)
            {
                _logger.LogWarning(aex, ApiLogMessages.ValidationFailed, aex.Message);
                return BadRequest(ApiResponse.Fail(aex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiLogMessages.UpdateError, updateFeatureDto.FeatureId);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Fail(ApiMessages.UpdateError));
            }
        }
    }
}


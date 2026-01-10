using Ecommerce.Catalog.Dtos.FeatureSliderDtos;
using Ecommerce.Catalog.Services.CategoryServices;
using Ecommerce.Catalog.Services.FeatureSliderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Common;

namespace Ecommerce.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")] 
    [ApiController]
    public class FeatureSlidersController : ControllerBase
    {
        private readonly IFeatureSliderService _featureSliderService;

        public FeatureSlidersController(IFeatureSliderService featureSliderService)
        {
            _featureSliderService = featureSliderService;
        }

        [HttpGet]
        public async Task<IActionResult> CategoryList()
        {
            var values = await _featureSliderService.GetAllFeatureSliderAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultFeatureSliderDto>>.Ok(values));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var value = await _featureSliderService.GetByIdFeatureSliderAsync(id);
            if (value == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

            return Ok(ApiResponse<GetByIdFeatureSliderDto>.Ok(value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateFeatureSliderDto createFeatureSliderDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _featureSliderService.CreateFeatureSliderAsync(createFeatureSliderDto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            await _featureSliderService.DeleteFeatureSliderAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(UpdateFeatureSliderDto updateFeatureSliderDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _featureSliderService.UpdateFeatureSliderAsync(updateFeatureSliderDto);
            return NoContent();
        }
    }
}


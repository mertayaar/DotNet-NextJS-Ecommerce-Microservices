using Ecommerce.Catalog.Dtos.ProductDetailDtos;
using Ecommerce.Catalog.Services.ProductDetailServices;
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
    public class ProductDetailsController : ControllerBase
    {
        private readonly IProductDetailService _ProductDetailService;

        public ProductDetailsController(IProductDetailService ProductDetailService)
        {
            _ProductDetailService = ProductDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailList()
        {
            var values = await _ProductDetailService.GetAllProductDetailAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultProductDetailDto>>.Ok(values));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetailById(string id)
        {
            var value = await _ProductDetailService.GetByIdProductDetailAsync(id);
            if (value == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

            return Ok(ApiResponse<GetByIdProductDetailDto>.Ok(value));
        }

        [HttpGet("GetProductDetailByProductId/{id}")]
        public async Task<IActionResult> GetProductDetailByProductId(string id)
        {
            var value = await _ProductDetailService.GetByProductIdProductDetailAsync(id);
            if (value == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.NotFoundByKey, id)));

            return Ok(ApiResponse<GetByIdProductDetailDto>.Ok(value));
        }


        [HttpPost]
        public async Task<IActionResult> CreateProductDetail(CreateProductDetailDto createProductDetailDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _ProductDetailService.CreateProductDetailAsync(createProductDetailDto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductDetail(string id)
        {
            await _ProductDetailService.DeleteProductDetailAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductDetail(UpdateProductDetailDto updateProductDetailDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _ProductDetailService.UpdateProductDetailAsync(updateProductDetailDto);
            return NoContent();
        }
    }
}



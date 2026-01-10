using Ecommerce.Catalog.Dtos.ContactDtos;
using Ecommerce.Catalog.Services.ContactServices;
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
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService categoryService)
        {
            _contactService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> ContactList()
        {
            var values = await _contactService.GetAllContactAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultContactDto>>.Ok(values));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(string id)
        {
            var value = await _contactService.GetByIdContactAsync(id);
            if (value == null)
                return NotFound(ApiResponse.Fail(string.Format(ApiMessages.IdNotFound, id)));

            return Ok(ApiResponse<GetByIdContactDto>.Ok(value));
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(CreateContactDto createContactDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _contactService.CreateContactAsync(createContactDto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContact(string id)
        {
            await _contactService.DeleteContactAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContact(UpdateContactDto updateContactDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }

            await _contactService.UpdateContactAsync(updateContactDto);
            return NoContent();
        }
    }
}


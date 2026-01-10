using Ecommerce.Message.Dtos;
using Ecommerce.Message.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Common;

namespace Ecommerce.Message.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMessageController : ControllerBase
    {
        private readonly IUserMessageService _userMessageService;

        public UserMessageController(IUserMessageService userMessageService)
        {
            _userMessageService = userMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages()
        {
            var values = await _userMessageService.GetAllMessageAsync();
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultMessageDto>>.Ok(values));
        }

        [HttpGet("GetMessageOutbox/{senderId}")]
        public async Task<IActionResult> GetMessageOutbox(string senderId)
        {
            var values = await _userMessageService.GetOutboxMessageAsync(senderId);
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultOutboxMessageDto>>.Ok(values));
        }

        [HttpGet("GetMessageInbox/{receiverId}")]
        public async Task<IActionResult> GetMessageInbox(string receiverId)
        {
            var values = await _userMessageService.GetInboxMessageAsync(receiverId);
            if (values == null || values.Count == 0)
                return NoContent();

            return Ok(ApiResponse<List<ResultInboxMessageDto>>.Ok(values));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessageAsync(CreateMessageDto createMessageDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }
            await _userMessageService.CreateMessageAsync(createMessageDto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(ApiMessages.Created));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMessageAsync(int id)
        {
            await _userMessageService.DeleteMessageAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMessageAsync(UpdateMessageDto updateMessageDto)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.Fail(modelErrors));
            }
            await _userMessageService.UpdateMessageAsync(updateMessageDto);
            return NoContent();
        }

        [HttpGet("TotalMessageCount")]
        public async Task<IActionResult> GetTotalMessageCount()
        {
            var values = await _userMessageService.GetTotalMessageCount();
            return Ok(ApiResponse<int>.Ok(values));
        }

        [HttpGet("TotalMessageCountByReceiverId/{id}")]
        public async Task<IActionResult> GetTotalMessageCountByReceiverId(string id)
        {
            var values = await _userMessageService.GetTotalMessageCountByReceiverId(id);
            return Ok(ApiResponse<int>.Ok(values));
        }

    }
}


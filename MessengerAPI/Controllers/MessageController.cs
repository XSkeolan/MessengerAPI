using MessengerAPI.DTOs;
using MessengerAPI.Models;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private/messages")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [Authorize]
        [Route("sendMessage")]
        public async Task<IActionResult> SendMessage(MessageRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(ResponseErrors.EMPTY_MESSAGE);
            }

            try
            {
                Message message = new Message
                {
                    DateSend = DateTime.UtcNow,
                    Destination = request.Destination,
                    Text = request.Message,
                    ReplyMessageId = request.ReplyMessageId,
                    OriginalMessageId = null
                };
                MessageResponse response = await _messageService.SendMessageAsync(message);
                return Created($"api/private/messages?id={response.MessageId}", response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("getMessages")]
        public async Task<IActionResult> GetMessages(IEnumerable<Guid> ids)
        {
            try
            {
                return Ok(/*await _messageService.GetMessagesAsync(userId, ids)*/);
            }
            catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}

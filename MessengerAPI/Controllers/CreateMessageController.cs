using MessengerAPI.DTOs;
using MessengerAPI.Models;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private")]
    [ApiController]
    public class CreateMessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public CreateMessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [Authorize]
        [Route("sendMessage")]
        public async Task<IActionResult> SendMessage(MessageRequest request)
        {
            if (request.Message == string.Empty)
            {
                return BadRequest(ResponseErrors.EMPTY_MESSAGE);
            }

            Guid userId = Guid.Parse(HttpContext.Items["User"].ToString());

            try
            {
                Message message = new Message
                {
                    DateSend = DateTime.UtcNow,
                    From = userId,
                    Destination = request.Destination,
                    DestinationType = request.DestinationType,
                    Text = request.Message,
                    ReplyMessageId = request.ReplyMessageId
                };
                return Ok(await _messageService.SendMessageAsync(message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

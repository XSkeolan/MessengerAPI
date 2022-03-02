using MessengerAPI.DTOs;
using MessengerAPI.Models;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateMessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public CreateMessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageRequest request)
        {
            if(request.Message == string.Empty)
            {
                return BadRequest(ResponseErrors.EMPTY_MESSAGE);
            }

            try
            {
                Message message = new Message 
                { 
                    DateSend = DateTime.Now,
                    From = request.From,
                    Destination =request.Destination,
                    DestinationType = request.DestinationType,
                    Text = request.Message,
                    OriginalMessageId = request.ReplyMessageId 
                };
                return Ok(await _messageService.SendMessageAsync(message));
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

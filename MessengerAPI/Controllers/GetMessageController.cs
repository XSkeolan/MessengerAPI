using MessengerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public GetMessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMessages(Guid chatid)
        {
            return Ok(await _messageService.GetMessagesAsync(chatid));
        }
    }
}

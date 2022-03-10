using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private")]
    [ApiController]
    public class GetMessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public GetMessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        [Authorize]
        [Route("getMessages")]
        public async Task<IActionResult> GetMessages(IEnumerable<Guid> ids)
        {
            Guid userId = Guid.Parse(HttpContext.Items["User"].ToString());
            try
            {
                return Ok(await _messageService.GetMessagesAsync(userId, ids));
            }
            catch(Exception ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}

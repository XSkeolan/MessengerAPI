using MessengerAPI.DTOs;
using MessengerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/createChat")]
    [ApiController]
    public class CreateChatController : ControllerBase
    {
        
        [HttpPost]
        public IActionResult CreateChat(ChatRequest request)
        {
            if(request.Name == string.Empty)
                return BadRequest(ResponseErrors.INVALID_FIELDS);

            Chat chat = new Chat { Name=request.Name,  };

        }
    }
}

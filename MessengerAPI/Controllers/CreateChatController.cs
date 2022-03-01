using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/createChat")]
    [ApiController]
    public class CreateChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public CreateChatController(IChatService chatService)
        {
            _chatService = chatService;
        }
        [HttpPost]
        public IActionResult CreateChat(ChatRequest request)
        {
            if (request.Name == string.Empty || request.InviteUsers.Length == 0)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            if (request.InviteUsers.Length == 1 && request.InviteUsers[0] == request.AdministratorId)
            {
                return BadRequest();
            }

            if (request.Photo != null)
            {
                using (Stream sw = new FileStream("", FileMode.CreateNew))
                {
                    sw.Write(request.Photo, 0, request.Photo.Length);
                }
            }
            Chat chat = new Chat { Name=request.Name, Description = request.Description, Administrator = request.AdministratorId, Photo = new InputFile("",""), Created = DateTime.Now };
            _chatService.CreateChat(chat, request.InviteUsers);
            return Ok();
        }
    }
}

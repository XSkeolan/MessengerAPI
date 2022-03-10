using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private")]
    [ApiController]
    public class CreateChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public CreateChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [Authorize]
        [Route("createChat")]
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            if (request.Name == string.Empty || request.InviteUsers.Length == 0)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            Guid userId = Guid.Parse(HttpContext.Items["User"].ToString());
            if (request.InviteUsers.Contains(userId))
            {
                return BadRequest(ResponseErrors.INVALID_INVITE_USER);
            }

            //if (request.Photo != null)
            //{
            //    using (Stream sw = new FileStream("", FileMode.CreateNew))
            //    {
            //        sw.Write(request.Photo, 0, request.Photo.Length);
            //    }
            //}
            Chat chat = new Chat 
            { 
                Name=request.Name, 
                Description = request.Description, 
                Administrator = userId, 
                Photo = new InputFile("", ""), 
                Created = DateTime.UtcNow 
            };
            return Ok(await _chatService.CreateChat(chat, request.InviteUsers.Distinct().ToArray()));
        }
    }
}

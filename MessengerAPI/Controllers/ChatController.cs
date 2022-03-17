using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [Authorize]
        [Route("createChat")]
        public async Task<IActionResult> CreateChat(ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || request.InviteUsers.Length < 2)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
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
                Photo = null, 
                Type = request.ChatType,
                Created = DateTime.UtcNow 
            };
            
            await _chatService.CreateChat(chat);
            ChatCreateResponse response = new ChatCreateResponse
            {
                ChatType = chat.Type,
                Description = chat.Description,
                Name = chat.Name,
                ChatId = chat.Id,
                InviteUsers = await _chatService.InviteUsersAsync(chat.Id, request.InviteUsers)
            };

            return Created("api/private/chat", response);
        }

        [HttpGet]
        [Authorize]
        [Route("getHistoryChat")]
        public async Task<IActionResult> GetChatHistory(Guid chatId)
        {
            return Ok();
        }
    }
}

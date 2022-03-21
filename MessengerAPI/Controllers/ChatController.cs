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
            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                ChatType = chat.Type,
                Description = chat.Description,
                Name = chat.Name,
                InviteUsers = await _chatService.InviteUsersAsync(chat.Id, request.InviteUsers)
            };

            return Created($"api/private/chat?id={response.ChatId}", response);
        }

        [HttpGet]
        [Authorize]
        [Route("getHistoryChat")]
        public async Task<IActionResult> GetChatHistory(Guid chatId)
        {
            return Ok();
        }

        [HttpGet("getChat")]
        [Authorize]
        public async Task<IActionResult> GetChats(Guid id)
        {
            return Ok(await _chatService.GetChatAsync(id));
        }
    }
}

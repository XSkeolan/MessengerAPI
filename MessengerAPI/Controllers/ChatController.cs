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
            if (string.IsNullOrEmpty(request.Name))
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
                Name = request.Name,
                Description = request.Description,
                Photo = null,
                Created = DateTime.UtcNow
            };

            await _chatService.CreateChatAsync(chat);
            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Description = chat.Description,
                Name = chat.Name,
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
        public async Task<IActionResult> GetChat(Guid id)
        {
            try
            {
                return Ok(await _chatService.GetChatAsync(id));
            }
            catch(SystemException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("editName")]
        [Authorize]
        public async Task<IActionResult> EditName(Guid chatId, string name)
        {
            try
            {
                ChatResponse? response = await _chatService.EditNameAsync(chatId, name);

                if (response == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok();
                }
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("editDescription")]
        [Authorize]
        public async Task<IActionResult> EditDescription(Guid chatId, string name)
        {
            try
            {
                ChatResponse? response = await _chatService.EditDescriptionAsync(chatId, name);

                if (response == null)
                {
                    return NoContent();
                }
                else
                {
                    return Ok();
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("editAdmin")]
        [Authorize]
        public async Task<IActionResult> EditAdmin(Guid chatId, Guid userId)
        {
            return Ok();
        }

        [HttpDelete("deleteChat")]
        [Authorize]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            try
            {
                await _chatService.DeleteChatAsync(chatId);
                return NoContent();
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

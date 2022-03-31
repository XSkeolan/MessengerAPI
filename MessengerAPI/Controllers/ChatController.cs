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

            Guid creatorId = await _chatService.CreateChatAsync(chat);
            await _chatService.InviteUserAsync(chat.Id, creatorId);
            await _chatService.SetRole(chat.Id, creatorId, Guid.Parse("bb6dc5a0-9546-438b-ac19-00a748b2be82"));

            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Description = chat.Description,
                Name = chat.Name,
            };

            return Created($"api/private/chat?id={response.ChatId}", response);
        }

        [HttpPost("inviteInChat")]
        [Authorize]
        public async Task<IActionResult> InviteInChat(InviteToChatRequest request)
        {
            if(!await _chatService.ChatIsAvaliableAsync(request.ChatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }
            if (!request.InviteUsers.Any())
            {
                return Ok();
            }

            IEnumerable<ShortUserResponse> responses = new List<ShortUserResponse>();
            foreach (Guid user in request.InviteUsers)
            {
                try
                {
                    responses = responses.Append(await _chatService.InviteUserAsync(request.ChatId, user));
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Ok(responses);
        }

        [HttpPut("kickUserFromChat")]
        [Authorize]
        public async Task<IActionResult> KickUserFromChat(KickUserFromChatRequest request)
        {
            if(!await _chatService.ChatIsAvaliableAsync(request.ChatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }
            if(!request.KickUsers.Any())
            {
                return Ok();
            }

            foreach (KickUserRequest kickUser in request.KickUsers)
            {
                try
                {
                    await _chatService.KickUserAsync(request.ChatId, kickUser.UserId);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok();
        }

        [HttpGet("getHistoryChat")]
        [Authorize]
        public async Task<IActionResult> GetChatHistory(Guid chatId)
        {
            if (await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

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
            if(!await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            return Ok();
        }

        [HttpPut("editDescription")]
        [Authorize]
        public async Task<IActionResult> EditDescription(Guid chatId, string newDescription)
        {
            if(await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            await _chatService.EditDescriptionAsync(chatId, newDescription);

            return Ok();
        }

        [HttpPut("editAdmin")]
        [Authorize]
        public async Task<IActionResult> EditAdmin(Guid chatId, Guid userId)
        {
            if (!await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            return Ok();
        }

        [HttpDelete("deleteChat")]
        [Authorize]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            if(!await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            try
            {
                await _chatService.DeleteChatAsync(chatId);
                return Ok();
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("setRole")]
        [Authorize]
        public async Task<IActionResult> SetRole(Guid chatId, Guid userId, Guid roleId)
        {
            if(! await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            try
            {
                await _chatService.SetRole(chatId, userId, roleId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _chatService.GetRoles());
        }
    }
}

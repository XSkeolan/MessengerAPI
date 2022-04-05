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

            Chat chat = new Chat
            {
                Name = request.Name,
                Description = request.Description,
                PhotoId = null,
                Created = DateTime.UtcNow
            };

            await _chatService.CreateChatAsync(chat);
            await _chatService.InviteUserAsync(chat.Id, chat.CreatorId);
            await _chatService.SetRoleAsync(chat.Id, chat.CreatorId, Guid.Parse("bb6dc5a0-9546-438b-ac19-00a748b2be82"));

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
        public async Task<IActionResult> InviteInChat(InviteUserRequest request)
        {
            if (!await _chatService.ChatIsAvaliableAsync(request.ChatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            try
            {
                await _chatService.InviteUserAsync(request.ChatId, request.UserId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPut("kickUserFromChat")]
        [Authorize]
        public async Task<IActionResult> KickUserFromChat(KickUserRequest request)
        {
            if (!await _chatService.ChatIsAvaliableAsync(request.ChatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            try
            {
                await _chatService.KickUserAsync(request.ChatId, request.UserId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        //[HttpGet("getHistoryChat")]
        //[Authorize]
        //public async Task<IActionResult> GetChatHistory(Guid chatId)
        //{
        //    if (await _chatService.ChatIsAvaliableAsync(chatId))
        //    {
        //        return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
        //    }

        //    return Ok();
        //}

        [HttpGet("getChat")]
        [Authorize]
        public async Task<IActionResult> GetChat(Guid id)
        {
            try
            {
                return Ok(await _chatService.GetChatAsync(id));
            }
            catch (SystemException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("editName")]
        [Authorize]
        public async Task<IActionResult> EditName(Guid chatId, string name)
        {
            if (!await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            return Ok();
        }

        [HttpPut("editDescription")]
        [Authorize]
        public async Task<IActionResult> EditDescription(Guid chatId, string newDescription)
        {
            if (await _chatService.ChatIsAvaliableAsync(chatId))
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
            if (!await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            try
            {
                await _chatService.DeleteChatAsync(chatId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("setRole")]
        [Authorize]
        public async Task<IActionResult> SetRole(RoleRequest roleRequest)
        {
            if (!await _chatService.ChatIsAvaliableAsync(roleRequest.ChatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            try
            {
                await _chatService.SetRoleAsync(roleRequest.ChatId, roleRequest.UserId, roleRequest.RoleId);
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
            return Ok(await _chatService.GetRolesAsync());
        }

        [HttpGet("searchUsers")]
        [Authorize]
        public async Task<IActionResult> SearchUsers(SearchUserInChatRequest userRequest)
        {
            if (!(userRequest.LimitResult > 0) || userRequest.SubString.Length == 0 || userRequest.SubString.Length > 20)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            if (!await _chatService.ChatIsAvaliableAsync(userRequest.ChatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            IEnumerable<BaseUserResponse> foundUsers = await _chatService.SearchUsersAsync(userRequest.ChatId, userRequest.SubString);
            return Ok(foundUsers.Take(userRequest.LimitResult));
        }

        [HttpPut("editPhoto")]
        [Authorize]
        public async Task<IActionResult> EditPhoto(Guid chatId, Guid fileId)
        {
            if(!await _chatService.ChatIsAvaliableAsync(chatId))
            {
                return BadRequest(ResponseErrors.CHAT_NOT_FOUND);
            }

            await _chatService.EditPhotoAsync(chatId, fileId);

            return Ok();
        }
    }
}

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
                PhotoId = null,
                DateCreated = DateTime.UtcNow
            };

            await _chatService.CreateChatAsync(chat);
            await _chatService.InviteUserAsync(chat.Id, chat.CreatorId);
            await _chatService.SetRoleAsync(chat.Id, chat.CreatorId, Guid.Parse("bb6dc5a0-9546-438b-ac19-00a748b2be82"));

            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Name = chat.Name,
            };

            return Created($"api/private/chat?id={response.ChatId}", response);
        }

        [HttpPost("inviteInChat")]
        [Authorize]
        public async Task<IActionResult> InviteInChat(InviteUserRequest request)
        {
            try
            {
                await _chatService.InviteUserAsync(request.ChatId, request.UserId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("kickUserFromChat")]
        [Authorize]
        public async Task<IActionResult> KickUserFromChat(KickUserRequest request)
        {
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

        [HttpGet("getChat")]
        [Authorize]
        public async Task<IActionResult> GetChat(Guid id)
        {
            try
            {
                Chat chat = await _chatService.GetChatAsync(id);
                ChatResponse chatResponse = new ChatResponse
                {
                    ChatId = chat.Id,
                    Name = chat.Name,
                    Photo = chat.PhotoId,
                    CountUsers = chat.CountUser
                };

                return Ok(chatResponse);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("editName")]
        [Authorize]
        public async Task<IActionResult> EditName(Guid chatId, string name)
        {
            try
            {
                await _chatService.EditNameAsync(chatId, name);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpDelete("deleteChat")]
        [Authorize]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
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

        [HttpPost("setRole")]
        [Authorize]
        public async Task<IActionResult> SetRole(RoleRequest roleRequest)
        {
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
            IEnumerable<RoleResponse> roleResponses = (await _chatService.GetRolesAsync()).Select(type => new RoleResponse
            {
                Id = type.Id,
                IsDefault = type.IsDefault,
                Name = type.TypeName,
                Permissions = type.Permissions.Split(';'),
                PriorityLevel = type.PriorityLevel
            });

            return Ok(roleResponses);
        }

        [HttpGet("searchUsers")]
        [Authorize]
        public async Task<IActionResult> SearchUsers(SearchUserInChatRequest userRequest)
        {
            if (!(userRequest.LimitResult > 0) || userRequest.SubString.Length == 0 || userRequest.SubString.Length > 20)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            IEnumerable<User> foundUsers = await _chatService.SearchUsersAsync(userRequest.ChatId, userRequest.SubString);

            return Ok(foundUsers.Take(userRequest.LimitResult).Select(user => new BaseUserResponse
            { 
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Nickname = user.Nickname
            }));
        }

        [HttpPost("editPhoto")]
        [Authorize]
        public async Task<IActionResult> EditPhoto(Guid chatId, Guid fileId)
        {
            await _chatService.EditPhotoAsync(chatId, fileId);

            return Ok();
        }
    }
}

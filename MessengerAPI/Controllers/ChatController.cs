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
        private readonly ITokenService _tokenService;
        private readonly IFileService _fileService;

        public ChatController(IChatService chatService, ITokenService tokenService, IFileService fileService)
        {
            _chatService = chatService;
            _tokenService = tokenService;
            _fileService = fileService;
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
                PhotoId = request.PhotoId,
                DateCreated = DateTime.UtcNow,
                DefaultUserTypeId = request.DefaultUserTypeId
            };

            await _chatService.CreateChatAsync(chat);

            await _chatService.InviteUserAsync(chat.Id, chat.CreatorId);
            await _chatService.SetRoleAsync(chat.Id, chat.CreatorId, (await _chatService.GetAdminRoleAsync()).Id);

            ChatResponse response = new ChatResponse
            {
                Id = chat.Id,
                Name = chat.Name,
                CountUsers = 1,
                CreatorId = chat.CreatorId
            };

            return Created($"api/private/chat?id={response.Id}", response);
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
                    Id = chat.Id,
                    Name = chat.Name,
                    Photo = chat.PhotoId,
                    CreatorId = chat.CreatorId,
                    CountUsers = chat.CountUser
                };

                return Ok(chatResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            IEnumerable<RoleResponse> roleResponses = (await _chatService.GetRolesAsync()).Select(type => new RoleResponse
            {
                Id = type.Id,
                IsDefault = type.IsDefault,
                Name = type.TypeName,
                Permissions = type.Permissions?.Split(';'),
                PriorityLevel = type.PriorityLevel
            });

            return Ok(roleResponses);
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

        [HttpGet("searchUsers")]
        [Authorize]
        public async Task<IActionResult> SearchUsers(Guid chatId, string nickname, int limitResult)
        {
            if (!(limitResult > 0) && nickname.Length < 20)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            IEnumerable<User> foundUsers = await _chatService.SearchUsersAsync(chatId, nickname);

            return Ok(foundUsers.Take(limitResult).Select(user => new BaseUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Nickname = user.Nickname
            }));
        }

        [HttpPost("editName")]
        [Authorize]
        public async Task<IActionResult> EditName(Guid chatId, string name)
        {
            try
            {
                await _chatService.EditNameAsync(chatId, name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("editDescription")]
        public async Task<IActionResult> ChangeDescription(Guid channelId, string newDescription)
        {
            try
            {
                await _chatService.EditDescriptionAsync(channelId, newDescription);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("editPhoto")]
        [Authorize]
        public async Task<IActionResult> EditPhoto(Guid chatId, Guid fileId)
        {
            await _chatService.EditPhotoAsync(chatId, fileId);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("changeCreator")]
        public async Task<IActionResult> ChangeCreator(Guid channelId, Guid newCreatorId)
        {
            try
            {
                await _chatService.ChangeCreatorAsync(channelId, newCreatorId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("join")]
        public async Task<IActionResult> Join(string chatId)
        {
            try
            {
                if (!Guid.TryParse(chatId, out Guid chat))
                {
                    return BadRequest(ResponseErrors.INVALID_FIELDS);
                }

                await _chatService.JoinAsync(chat);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("leave")]
        public async Task<IActionResult> Leave([FromBody] string chatId)
        {
            try
            {
                if (!Guid.TryParse(chatId, out Guid guid))
                {
                    return BadRequest(ResponseErrors.INVALID_FIELDS);
                }

                await _chatService.LeaveAsync(guid);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("createLinkOnChannel")]
        public async Task<IActionResult> CreateInviteLink(InvitationLinkRequest request)
        {
            try
            {
                if (!TimeSpan.TryParse(request.ValidTime, out TimeSpan timeSpan))
                {
                    return BadRequest(ResponseErrors.INVALID_FIELDS);
                }

                ChatLink channelLink = new ChatLink
                {
                    GroupId = request.ChannelId,
                    DateEnd = DateTime.UtcNow + timeSpan,
                    IsOneTime = request.IsOneTime
                };
                await _chatService.CreateInvitationLinkAsync(channelLink);

                return Ok(new InvitationLinkResponse
                {
                    Id = channelLink.Id,
                    Link = "http://localhost:5037/api/public/channel?token=" + await _tokenService.CreateInvitationToken(channelLink.Id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("deleteLinkOnChannel")]
        public async Task<IActionResult> DeleteInviteLink(Guid linkid)
        {
            try
            {
                await _chatService.DeleteInvitationLinkAsync(linkid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("joinFromLink")]
        public async Task<IActionResult> GetChatFromLink(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            try
            {
                Chat chat = await _chatService.JoinByLinkAsync(token);

                ChatResponse channelResponse = new ChatResponse
                {
                    Id = chat.Id,
                    CreatorId = chat.CreatorId,
                    Name = chat.Name,
                    Description = chat.Description
                };

                return Ok(channelResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("private/uploadChatImage")]
        [Authorize]
        public async Task<IActionResult> UploadChatImage(IFormFile imageData)
        {
            if (imageData.Length == 0)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            return Ok(await _fileService.UploadFile(imageData));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MessengerAPI.DTOs;
using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MessengerAPI.Controllers
{
    [Route("api/private/channels")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;
        private readonly ITokenService _tokenService;

        public ChannelController(IChannelService channelService, ITokenService tokenService)
        {
            _channelService = channelService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            Channel channel = new Channel
            {
                Name = request.Name,
                Description = request.Description,
                DateCreated = DateTime.UtcNow,
                PhotoId = request.PhotoId
            };

            await _channelService.CreateChannelAsync(channel);
            await _channelService.JoinAsync(channel.Id);

            return Created($"api/private/channel?id={channel.Id}", null);
        }

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetById(Guid channelId)
        //{
        //    try
        //    {
        //        return Ok(await _channelService.GetChannelAsync(channelId));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] Guid channelId)
        {
            try
            {
                await _channelService.DeleteChannelAsync(channelId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("rename")]
        public async Task<IActionResult> Rename(Guid channelId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            try
            {
                await _channelService.RenameChannelAsync(channelId, newName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("changeDescription")]
        public async Task<IActionResult> ChangeDescription(Guid channelId, string newDescription)
        {
            try
            {
                await _channelService.ChangeDescriptionAsync(channelId, newDescription);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("changeCreator")]
        public async Task<IActionResult> ChangeCreator(Guid channelId, Guid newCreatorId)
        {
            try
            {
                await _channelService.ChangeCreatorAsync(channelId, newCreatorId);
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
        public async Task<IActionResult> Join([FromBody] Guid channelId)
        {
            try
            {
                await _channelService.JoinAsync(channelId);
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
        public async Task<IActionResult> Leave([FromBody] Guid channelId)
        {
            try
            {
                await _channelService.LeaveAsync(channelId);
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
                ChannelLink channelLink = new ChannelLink
                {
                    ChannelId = request.ChannelId,
                    DateEnd = DateTime.UtcNow + timeSpan,
                    IsOneTime = request.IsOneTime
                };
                await _channelService.CreateInvitationLinkAsync(channelLink);
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
                await _channelService.DeleteInvitationLinkAsync(linkid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/api/public/channels/joinFromLink")]
        public async Task<IActionResult> GetChannelFromLink(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            try
            {
                Channel channel = await _channelService.JoinByLinkAsync(token);
                ChannelResponse channelResponse = new ChannelResponse
                {
                    Id = channel.Id,
                    CreatorId = channel.CreatorId,
                    Name = channel.Name,
                    Description = channel.Description
                };
                return Ok(channelResponse);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

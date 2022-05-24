using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MessengerAPI.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IUserChannelRepository _userChannelRepository;
        private readonly IChannelLinkRepository _channelLinkRepository;
        private readonly IServiceContext _serviceContext;

        public ChannelService(IChannelRepository channelRepository,
            IUserRepository userRepository,
            IFileRepository fileRepository,
            IUserChannelRepository userChannelRepository,
            IChannelLinkRepository channelLinkRepository,
            IServiceContext serviceContext)
        {
            _channelRepository = channelRepository;
            _userRepository = userRepository;
            _fileRepository = fileRepository;
            _userChannelRepository = userChannelRepository;
            _channelLinkRepository = channelLinkRepository;
            _serviceContext = serviceContext;
        }

        public async Task CreateChannelAsync(Channel channel)
        {
            channel.CreatorId = _serviceContext.UserId;
            await _channelRepository.CreateAsync(_channelRepository.EntityToDictionary(channel));
        }

        public async Task<Channel> GetChannelAsync(Guid channelId)
        {
            Channel? channel = await _channelRepository.GetAsync(channelId);
            if (channel == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_NOT_FOUND);
            }

            return channel;
        }

        public async Task DeleteChannelAsync(Guid channelId)
        {
            await GetChannelAsync(channelId);
            await CheckUserOnCreator(channelId);

            List<UserChannel> usersChannels = new List<UserChannel>(await _userChannelRepository.GetChannelUsersAsync(channelId));
            foreach (UserChannel user in usersChannels)
            {
                await _userChannelRepository.DeleteAsync(user.Id);
            }

            await _channelRepository.DeleteAsync(channelId);
        }

        public async Task<IEnumerable<Channel>> FindChannelByName(string channelName)
        {
            return await _channelRepository.GetChannelByNameAsync(channelName);
        }

        public async Task EditPhotoAsync(Guid channelId, Guid filePhotoId)
        {
            await GetChannelAsync(channelId);
            await _fileRepository.GetAsync(filePhotoId);

            await CheckUserOnCreator(channelId);

            await _channelRepository.UpdateAsync(channelId, "photoid", filePhotoId);
        }

        public async Task RenameChannelAsync(Guid channelId, string newName)
        {
            await GetChannelAsync(channelId);
            await _channelRepository.UpdateAsync(channelId, "name", newName);
        }

        public async Task ChangeDescriptionAsync(Guid channelId, string newDescription)
        {
            await GetChannelAsync(channelId);
            await _channelRepository.UpdateAsync(channelId, "description", newDescription);
        }

        public async Task ChangeCreatorAsync(Guid channelId, Guid userId)
        {
            await GetChannelAsync(channelId);
            await CheckUserOnCreator(channelId);

            User? user = await _userRepository.GetAsync(userId);

            if (user == null)
            {
                throw new ArgumentNullException(ResponseErrors.USER_NOT_FOUND);
            }
            else
            {
                if (!(await _userChannelRepository.GetChannelUsersAsync(channelId)).Select(x => x.UserId).Contains(userId))
                {
                    throw new ArgumentNullException(ResponseErrors.USER_NOT_PARTICIPANT);
                }
            }

            await _channelRepository.UpdateAsync(channelId, "creatorid", userId);
        }

        public async Task<Channel> JoinAsync(Guid channelId)
        {
            await GetChannelAsync(channelId);
            if ((await _userChannelRepository.GetChannelUsersAsync(channelId)).Select(x => x.Id).Contains(_serviceContext.UserId))
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_IN_CHANNEL);
            }

            UserChannel userChannel = new UserChannel
            {
                ChannelId = channelId,
                UserId = _serviceContext.UserId
            };

            await _userChannelRepository.CreateAsync(_userChannelRepository.EntityToDictionary(userChannel));

            return await _channelRepository.GetAsync(channelId);
        }

        public async Task LeaveAsync(Guid channelId)
        {
            await GetChannelAsync(channelId);

            UserChannel? userChannel = (await _userChannelRepository.GetChannelUsersAsync(channelId)).Where(x => x.UserId == _serviceContext.UserId).FirstOrDefault();
            if (userChannel == null)
            {
                throw new ArgumentNullException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            await _userChannelRepository.DeleteAsync(userChannel.Id);
        }

        public async Task CreateInvitationLinkAsync(ChannelLink channelLink)
        {
            await GetChannelAsync(channelLink.ChannelId);
            await CheckUserOnCreator(channelLink.ChannelId);

            await _channelLinkRepository.CreateAsync(_channelLinkRepository.EntityToDictionary(channelLink));
        }

        public async Task DeleteInvitationLinkAsync(Guid channelLinkId)
        {
            ChannelLink? link = await _channelLinkRepository.GetAsync(channelLinkId);
            if (link == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_NOT_FOUND);
            }

            await GetChannelAsync(link.ChannelId);
            await CheckUserOnCreator(link.ChannelId);

            await _channelLinkRepository.DeleteAsync(channelLinkId);
        }

        public async Task<Channel> JoinByLinkAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            Guid channelLinkId = Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value);
            DateTime dateEnd = DateTime.Parse(tokenS.Claims.First(claim => claim.Type == "DateEnd").Value);

            ChannelLink? channelLink = await _channelLinkRepository.GetAsync(channelLinkId);
            if (channelLink == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_ALREADY_USED);
            }

            if (channelLink.DateEnd < DateTime.UtcNow.ToLocalTime())
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_INVALID);
            }

            if (channelLink.IsOneTime)
            {
                await _channelLinkRepository.DeleteAsync(channelLinkId);
            }

            return await _channelRepository.GetAsync(channelLink.ChannelId);
        }

        private async Task CheckUserOnCreator(Guid channelId)
        {
            Channel? channel = await _channelRepository.GetAsync(channelId);

            if (channel.CreatorId != _serviceContext.UserId)
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }
        }
    }
}

using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class LinkService : ILinkService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILinkRepository _linkRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IServiceContext _serviceContext;

        public LinkService(IUserRepository userRepository, ILinkRepository linkRepository, IChatRepository chatRepository, IServiceContext serviceContext)
        {
            _userRepository = userRepository;
            _linkRepository = linkRepository;
            _chatRepository = chatRepository;
            _serviceContext = serviceContext;
        }

        public async Task<string> GetEmailLink(string emailToken)
        {
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException(ResponseErrors.USER_EMAIL_NOT_SET);
            }

            return "api/auth/confirm" + "?e=" + emailToken;
        }

        public async Task<string> GetChannelLink(Guid channelId, bool oneTime)
        {
            Chat? chat = await _chatRepository.GetAsync(channelId);
            if(chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            Link link = new Link
            {
                DateCreate = DateTime.UtcNow,
                GroupId = channelId,
                OneTime = oneTime,
                IsActive = true
            };

            return "";
        }
    }
}

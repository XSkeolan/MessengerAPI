using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class LinkService : ILinkService
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IServiceContext _serviceContext;

        public LinkService(IUserRepository userRepository, IChatRepository chatRepository, IServiceContext serviceContext)
        {
            _userRepository = userRepository;
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
    }
}

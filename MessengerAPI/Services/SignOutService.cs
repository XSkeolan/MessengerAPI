using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class SignOutService : ISignOutService
    {
        private readonly ISessionRepository _sessionRepository;

        public SignOutService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task SignOut(Guid sessionId, Guid userId)
        {
            Session? session = await _sessionRepository.GetAsync(sessionId);
            if (session == null)
                throw new ArgumentException(ResponseErrors.SESSION_NOT_FOUND);

            if (session.UserId != userId)
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);

            await _sessionRepository.DeleteAsync(sessionId);
        }
    }
}

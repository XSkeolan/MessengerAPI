using MessengerAPI.Interfaces;

namespace MessengerAPI.Services
{
    public class SessionTokenService : ISessionTokenService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionTokenService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<bool> SessionExists(Guid sessionId)
        {
            return await _sessionRepository.GetAsync(sessionId) == null;
        }

        public async Task<Guid> GetOwnerSession(Guid sessionId)
        {
            if (!await SessionExists(sessionId))
                throw new ArgumentException(ResponseErrors.SESSION_NOT_FOUND);

            return (await _sessionRepository.GetAsync(sessionId)).UserId;
        }
    }
}

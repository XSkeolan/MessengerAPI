using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class SignOutService : ISignOutService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IServiceContext _serviceContext;

        public SignOutService(ISessionRepository sessionRepository, IServiceContext serviceContext)
        {
            _sessionRepository = sessionRepository;
            _serviceContext = serviceContext;
        }

        public async Task SignOut()
        {
            await _sessionRepository.UpdateAsync(_serviceContext.SessionId, DateTime.UtcNow);
        }
    }
}

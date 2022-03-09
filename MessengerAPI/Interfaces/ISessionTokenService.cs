
namespace MessengerAPI.Services
{
    public interface ISessionTokenService
    {
        Task<Guid> GetOwnerSession(Guid sessionId);
        Task<bool> SessionExists(Guid sessionId);
    }
}
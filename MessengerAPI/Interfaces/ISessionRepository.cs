using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISessionRepository
    {
        Task CreateAsync(Session session);
        Task FinishSessionAsync(Guid id);
        Task<Session?> GetAsync(Guid id);
    }
}
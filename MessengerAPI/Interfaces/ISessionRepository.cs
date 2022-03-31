using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<Session?> GetUnfinishedOnDeviceAsync(string device);
        Task UpdateAsync(Guid id, DateTime dateEnd);
    }
}
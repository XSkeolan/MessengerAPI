using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISessionRepository : IRepository<Session>
    {
        public Task<Session?> GetUnfinishedOnDeviceAsync(string device);
    }
}
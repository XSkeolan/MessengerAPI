using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserChannelRepository : IRepository<UserChannel>
    {
        Task<IEnumerable<UserChannel>> GetChannelUsersAsync(Guid channelId);
    }
}
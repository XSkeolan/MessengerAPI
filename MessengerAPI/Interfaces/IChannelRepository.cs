using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChannelRepository : IRepository<Channel>
    {
        Task<IEnumerable<Channel?>> GetChannelByNameAsync(string name);
    }
}
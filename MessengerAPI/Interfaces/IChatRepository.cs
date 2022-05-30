using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatRepository : IRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetChatsByNameAsync(string name);
    }
}

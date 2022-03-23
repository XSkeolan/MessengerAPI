using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatRepository : IRepository<Chat>
    {
        Task UpdateAsync(Guid id, string name, string description);
        Task<Message?> GetLastMessage(Guid id);
    }
}

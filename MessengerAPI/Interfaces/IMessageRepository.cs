using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId);
        Task<Message?> GetLastMessage(Guid destinationId);
        Task UpdateAsync(Guid id, bool isPinned);
        Task UpdateAsync(Guid id, string text);
        Task UpdateAsync(Guid id);
    }
}

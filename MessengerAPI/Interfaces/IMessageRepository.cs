using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId);
        Task<Message?> GetLastMessage(Guid destinationId);
    }
}

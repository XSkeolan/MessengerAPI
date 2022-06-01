using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IMessageFileRepository : IRepository<MessageFile>
    {
        Task<IEnumerable<MessageFile>> GetMessageFiles(Guid messageId);
    }
}

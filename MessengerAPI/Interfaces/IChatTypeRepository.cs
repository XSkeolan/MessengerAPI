using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatTypeRepository : IRepository<ChatType>
    {
        Task<ChatType> GetByTypeName(string type);
    }
}
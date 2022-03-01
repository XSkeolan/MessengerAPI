using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserTypeRepository : IRepository<UserType>
    {
        Task<Guid> GetIdByTypeName(string typeName);
    }
}

using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserTypeRepository : IRepository<UserType>
    {
        Task<UserType?> GetByTypeNameAsync(string typeName);
        Task<UserType> GetDefaultType();
        Task<IEnumerable<UserType>> GetAll();
    }
}

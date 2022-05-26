using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserTypeRepository : IRepository<UserType>
    {
        Task<UserType?> GetByTypeNameAsync(string typeName);
        Task<IEnumerable<UserType>> GetDefaultTypes();
        Task<IEnumerable<UserType>> GetAll();
        Task<UserType> GetByMaxPriorityAsync();
    }
}

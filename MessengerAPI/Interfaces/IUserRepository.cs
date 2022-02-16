using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<User?> GetAsync(Guid id);
        Task UpdateAsync(User user);
        Task<User?> FindByPhonenumberAsync(string phonenumber);
        Task<User?> FindByNicknameAsync(string nickname);
    }
}

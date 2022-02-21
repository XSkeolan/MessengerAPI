using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByPhonenumberAsync(string phonenumber);
        Task<User?> FindByNicknameAsync(string nickname);
    }
}

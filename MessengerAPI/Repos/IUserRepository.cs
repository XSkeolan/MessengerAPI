using MessengerAPI.Models;

namespace MessengerAPI.Repos
{
    public interface IUserRepository
    {
        void Create(User user);
        void Delete(Guid id);
        User? Get(Guid id);
        void Update(User user);
        User? FindByPhonenumber(string phonenumber);
    }
}

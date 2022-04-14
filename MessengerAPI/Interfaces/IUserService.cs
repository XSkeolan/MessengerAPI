using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public interface IUserService
    {
        Task<User> GetUserByPhonenumber(string phoneNumber);
        Task<IEnumerable<User>> SearchUsersGlobal(string nickname);
        Task SignUp(User user, string password);
        Task SignOut();
        Task<SignInResponseUserInfo> SignIn(string phonenumber, string password, string deviceName);
        Task ChangePassword(string newPassword);
        Task<bool> CheckCode(string code);
    }
}
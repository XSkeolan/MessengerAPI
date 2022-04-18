using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public interface IUserService
    {
        public int TokenExpires { get; }
        public int EmailCodeExpires { get; }

        Task SignUp(User user, string password);
        Task<string> SignIn(string phonenumber, string password, string deviceName);
        Task SignOut();

        Task<User> GetUserByPhonenumber(string phoneNumber);
        Task<IEnumerable<User>> SearchUsersGlobal(string nickname);
        Task ChangePassword(string newPassword);
        Task<User?> GetUser(Guid id);
        Task DeleteUser(string reason);
        Task UpdateStatus(string status);
        Task<bool> ConfirmEmail(string code);
        Task SendCodeAsync();
        Task ResendCodeAsync();
        Task<bool> CheckCodeAsync(string code);
    }
}
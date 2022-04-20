using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserService
    {
        public int SessionExpires { get; }
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
        Task SendToEmailAsync(string subject, string content);
        Task UpdateUser();
        Task<string> CreateEmailToken();
    }
}
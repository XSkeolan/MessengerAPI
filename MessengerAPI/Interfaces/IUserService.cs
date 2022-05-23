using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserService
    {
        public int SessionExpires { get; }
        public int EmailCodeExpires { get; }

        Task SignUp(User user, string password);
        Task<Session> SignIn(string phonenumber, string password, string deviceName);
        Task SignOut();

        Task<User> GetUserByPhonenumber(string phoneNumber);
        Task<User?> GetCurrentUser();
        Task<IEnumerable<User>> SearchUsersGlobal(string nickname);
        Task ChangePassword(Guid? userid, string password);
        Task<User?> GetUser(Guid id);
        Task DeleteUser(string reason);
        Task UpdateStatus(string status);
        Task<bool> ConfirmEmail(string code);
        Task SendCodeAsync(string phonenumber);
        Task ResendCodeAsync(string email);
        Task<ConfirmationCode> TryGetCodeInfoAsync(string code);
        Task UpdateUserInfo(string name, string surname, string nickname, string email);
        Task SendToEmailAsync(string email, string subject, string content);
    }
}
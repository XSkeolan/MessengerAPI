using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISignUpService
    {
        Task<SignInResponseUserInfo> SignUp(User user, string enteringDeviceName, string password);
    }
}

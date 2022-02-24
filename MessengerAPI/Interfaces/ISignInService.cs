
using MessengerAPI.DTOs;

namespace MessengerAPI.Interfaces
{
    public interface ISignInService
    {
        Task<SignInResponseUserInfo> SignIn(string phonenumber, string password);
    }
}
using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISignUpService
    {
        Task<SignUpResponseUserInfo> SignUp(User user, Session device);
    }
}

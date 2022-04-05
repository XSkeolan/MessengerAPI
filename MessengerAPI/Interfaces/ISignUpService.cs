using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISignUpService
    {
        Task<UserCreateResponse> SignUp(User user, string password);
    }
}

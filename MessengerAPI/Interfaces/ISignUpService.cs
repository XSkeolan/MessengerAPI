using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface ISignUpService
    {
        Task<UserResponse> SignUp(User user, string password);
    }
}

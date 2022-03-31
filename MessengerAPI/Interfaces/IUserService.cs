using MessengerAPI.DTOs;

namespace MessengerAPI.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetUserByPhonenumber(string phoneNumber);
    }
}
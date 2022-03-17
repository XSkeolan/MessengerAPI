using MessengerAPI.DTOs;

namespace MessengerAPI.Services
{
    public interface IUserService
    {
        Task<UserCreateResponse> GetUserByPhonenumber(string phoneNumber);
    }
}
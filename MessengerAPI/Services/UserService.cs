using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;

namespace MessengerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserResponse> GetUserByPhonenumber(string phoneNumber)
        {
            var user = await _userRepository.FindByPhonenumberAsync(phoneNumber);
            if (user == null)
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);

            return new UserResponse
            {
                Id = user.Id,
                IsConfirmed = user.IsConfirmed,
                Name = user.Name,
                Surname = user.Surname,
                IsDeleted = user.IsDeleted,
                Nickname = user.Nickname,
                Phonenumber = user.Phonenumber
            };
        }
    }
}

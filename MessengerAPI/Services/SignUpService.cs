using MessengerAPI.Models;
using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;

namespace MessengerAPI.Services
{
    public class SignUpService: ISignUpService
    {
        private readonly IUserRepository _userRepository;

        public SignUpService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> SignUp(User user, string password)
        {
            if (await _userRepository.FindByPhonenumberAsync(user.Phonenumber) != null ||
                await _userRepository.FindByNicknameAsync(user.Nickname) != null)
            {
                throw new ArgumentException(ResponseErrors.ALREADY_EXISTS);
            }

            user.Password = Password.GetHasedPassword(password);
            await _userRepository.CreateAsync(user);

            UserResponse responseUser = new UserResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Name = user.Name,
                Surname = user.Surname,
                Phonenumber = user.Phonenumber,
                IsConfirmed = false
            };

            return responseUser;
        }
    }
}
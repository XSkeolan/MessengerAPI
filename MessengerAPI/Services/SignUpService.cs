using MessengerAPI.Models;
using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;

namespace MessengerAPI.Services
{
    public class SignUpService: ISignUpService
    {
        private readonly IUserRepository _users;
        private readonly ISessionRepository _sessions;

        public SignUpService(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            _users = userRepository;
            _sessions = sessionRepository;
        }

        public async Task<UserResponse> SignUp(User user, string enteringDeviceName, string password)
        {
            if(await _users.FindByPhonenumberAsync(user.Phonenumber) != null || 
                await _users.FindByNicknameAsync(user.Nickname) != null)
                throw new ArgumentException(ResponseErrors.ALREADY_EXISTS);

            user.Password = Password.GetHasedPassword(password);
            await _users.CreateAsync(user);

            UserResponse responseUser = new UserResponse
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Name = user.Name,
                Surname = user.Surname,
                Phonenumber = user.Phonenumber,
                IsDeleted = false,
                IsConfirmed = false
            };

            return responseUser;
        }
    }
}
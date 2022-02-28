using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class SignInService : ISignInService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public SignInService(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<SignInResponseUserInfo> SignIn(string phonenumber, string password)
        {
            User? user = await _userRepository.FindByPhonenumberAsync(phonenumber);
            if (user == null)
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);

            Password.VerifyHashedPassword(user.Password, password);

            Session session = new Session { DateStart = DateTime.Now, UserId = user.Id };
            await _sessionRepository.CreateAsync(session);

            return new SignInResponseUserInfo
            {
                SessionId = session.Id,
                User = new UserResponse
                {
                    Id = user.Id,
                    Nickname = user.Nickname,
                    Name = user.Name,
                    Surname = user.Surname,
                    Phonenumber = user.Phonenumber,
                    IsDeleted = user.IsDeleted,
                    IsConfirmed = user.IsConfirmed
                }
            };
        }
    }
}

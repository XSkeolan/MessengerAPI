using MessengerAPI.Models;
using MessengerAPI.Repositories;
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

        public async Task<SignUpResponseUserInfo> SignUp(User user, Session session)
        {
            if(await _users.FindByPhonenumberAsync(user.Phonenumber) != null)
                throw new ArgumentException(ResponseErrors.PHONENUMBER_ALREADY_EXISTS);

            if(await _users.FindByNicknameAsync(user.Nickname) != null)
                throw new ArgumentException(ResponseErrors.NICKNAME_ALREADY_EXISTS);

            await _users.CreateAsync(user);

            session.UserId = user.Id;
            await _sessions.CreateAsync(session);

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

            return new SignUpResponseUserInfo { SessionId = session.Id, User = responseUser };
        }
    }
}
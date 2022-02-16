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

        public SignUpService(IConfiguration configuration)
        {
            _users = new UserRepository(configuration.GetConnectionString("MessengerAPI"));
            _sessions = new SessionRepository(configuration.GetConnectionString("MessengerAPI"));
        }

        public async Task<SignUpResponseUserInfo> SignUp(User user, Session session)
        {
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
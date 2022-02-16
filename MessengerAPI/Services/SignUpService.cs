using MessengerAPI.Models;
using MessengerAPI.Repos;
using MessengerAPI.DTOs;

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
        public SignUpResponse SignUp(SignUpRequest inputUser, string device)
        {
            User user = _users.FindByPhonenumber(inputUser.Phonenumber) == null ? new User
            {
                Password = inputUser.Password,
                Phonenumber = inputUser.Phonenumber,
                Name = inputUser.Name,
                Surname = inputUser.Surname,
                Nickname = inputUser.Nickname
            } : throw new ArgumentException("User with this phonenumber already exists");
            _users.Create(user);
            Session session = new Session { UserId = user.Id, DateStart = DateTime.Now, DeviceName = device };
            _sessions.Create(session);
            UserResponse responseUser = new UserResponse
            { 
                Id = user.Id, 
                Nickname=user.Nickname, 
                Name= user.Name,
                Surname= user.Surname,
                Phonenumber = user.Phonenumber,
                IsDeleted = false,
                IsConfirmed = false
            };
            return new SignUpResponse { SessionId=session.Id, User = responseUser };
        }
    }
}
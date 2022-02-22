using MessengerAPI.Models;
using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using System.Security.Cryptography;

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

        public async Task<SignUpResponseUserInfo> SignUp(User user, string enteringDeviceName, string password)
        {
            if(await _users.FindByPhonenumberAsync(user.Phonenumber) != null || 
                await _users.FindByNicknameAsync(user.Nickname) != null)
                throw new ArgumentException(ResponseErrors.ALREADY_EXISTS);

            byte[] salt = new byte[16];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            user.HashedPassword = Convert.ToBase64String(hashBytes);

            await _users.CreateAsync(user);

            Session session = new Session { UserId = user.Id, DateStart=DateTime.Now, DeviceName = enteringDeviceName };
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
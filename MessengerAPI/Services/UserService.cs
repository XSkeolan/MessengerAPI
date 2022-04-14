using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessengerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IServiceContext _serviceContext;
        private readonly ISessionRepository _sessionRepository;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _expires;

        public UserService(IUserRepository userRepository, IUserChatRepository userChatRepository, IServiceContext serviceContext, ISessionRepository sessionRepository, IOptions<JWTOptions> options)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _serviceContext = serviceContext;
            _sessionRepository = sessionRepository;

            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _key = options.Value.Key;
            _expires = options.Value.Expires;
        }

        public async Task<User> GetUserByPhonenumber(string phoneNumber)
        {
            var user = await _userRepository.FindByPhonenumberAsync(phoneNumber);
            if (user == null)
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);

            return user;
        }

        public async Task<IEnumerable<User>> SearchUsersGlobal(string nickname)
        {
            return await _userRepository.FindByNicknameAsync(nickname);
        }

        public async Task<User?> GetUser(Guid id)
        {
            if(id != _serviceContext.UserId)
            {
                IEnumerable<Guid> firstUserChats = await _userChatRepository.GetUserChatsAsync(id);
                IEnumerable<Guid> secondUserChats = await _userChatRepository.GetUserChatsAsync(_serviceContext.UserId);

                if(!firstUserChats.Intersect(secondUserChats).Any())
                {
                    throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
                }
            }

            return await _userRepository.GetAsync(id);
        }

        public async Task DeleteUser(string reason)
        {
            await _userRepository.DeleteAsync(_serviceContext.UserId, reason);
        }
        
        public async Task ChangePassword(string password)
        {
            string hasedPassword = Password.GetHasedPassword(password);
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if(user.Password != hasedPassword)
            {
                await _userRepository.UpdateAsync(hasedPassword);
            }
        }

        public async Task UpdateStatus(string status)
        {
            await _userRepository.UpdateAsync(status);
        }

        public async Task ConfirmEmail()
        {
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if(!user.IsConfirmed)
            {

            }
        }

        public async Task SignUp(User user, string password)
        {
            if (await _userRepository.FindByPhonenumberAsync(user.Phonenumber) != null ||
                await _userRepository.FindByNicknameAsync(user.Nickname) != null)
            {
                throw new ArgumentException(ResponseErrors.ALREADY_EXISTS);
            }

            user.Password = Password.GetHasedPassword(password);
            await _userRepository.CreateAsync(user);
        }

        public async Task<SignInResponseUserInfo> SignIn(string phonenumber, string password, string deviceName)
        {
            User? user = await _userRepository.FindByPhonenumberAsync(phonenumber);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            if (await _sessionRepository.GetUnfinishedOnDeviceAsync(deviceName) != null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_AUTHORIZE);
            }

            Password.VerifyHashedPassword(user.Password, password);

            Session session = new Session
            {
                DateStart = DateTime.UtcNow,
                UserId = user.Id,
                DeviceName = deviceName,
                DateEnd = DateTime.UtcNow.AddSeconds(_expires)
            };
            await _sessionRepository.CreateAsync(session);

            var identity = GetIdentity(session);
            var jwt = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    notBefore: session.DateStart,
                    claims: identity.Claims,
                    expires: session.DateEnd,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new SignInResponseUserInfo
            {
                Token = encodedJwt,
                Expiries = _expires,
                User = new UserCreateResponse
                {
                    Id = user.Id,
                    Nickname = user.Nickname,
                    Name = user.Name,
                    Surname = user.Surname,
                    Phonenumber = user.Phonenumber,
                    IsConfirmed = user.IsConfirmed
                }
            };
        }

        private ClaimsIdentity GetIdentity(Session session)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, session.Id.ToString(), "Guid"),
                    new Claim("DateEnd", session.DateEnd.ToString(), "DateTime"),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"),
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        public async Task SignOut()
        {
            await _sessionRepository.UpdateAsync(_serviceContext.SessionId, DateTime.UtcNow);
        }
    }
}

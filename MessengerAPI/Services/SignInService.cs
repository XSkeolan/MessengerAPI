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
    public class SignInService : ISignInService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _expires;

        public SignInService(IUserRepository userRepository, ISessionRepository sessionRepository, IOptions<JWTOptions> options)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _key = options.Value.Key;
            _expires = options.Value.Expires;
        }

        public async Task<SignInResponseUserInfo> SignIn(string phonenumber, string password, string deviceName)
        {
            User? user = await _userRepository.FindByPhonenumberAsync(phonenumber);
            if (user == null)
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);

            Password.VerifyHashedPassword(user.Password, password);

            Session session = new Session { DateStart = DateTime.UtcNow, UserId = user.Id, DeviceName=deviceName, DateEnd = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_expires)) };
            await _sessionRepository.CreateAsync(session);

            var identity = GetIdentity(session);
            var jwt = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    notBefore: DateTime.UtcNow,
                    claims: identity.Claims,
                    expires: DateTime.UtcNow.AddSeconds(_expires),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new SignInResponseUserInfo
            {
                Token = encodedJwt,
                Expiries = _expires,
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
    }
}

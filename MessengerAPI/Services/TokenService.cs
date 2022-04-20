using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _sessionExpires;
        private readonly int _emailLinkExpires;

        public TokenService(IOptions<JwtOptions> options, ISessionRepository sessionRepository, IUserRepository userRepository) 
        {
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _key = options.Value.Key;
            _sessionExpires = options.Value.SessionExpires;
            _emailLinkExpires = options.Value.EmailLinkExpires;

            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
        }

        public async Task<string> CreateSessionToken(Guid sessionId)
        {
            Session? session = await _sessionRepository.GetAsync(sessionId);
            if(session == null)
            {
                throw new ArgumentException(ResponseErrors.SESSION_NOT_FOUND);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, session.Id.ToString(), "Guid"),
                new Claim("DateEnd", session.DateEnd.ToString(), "DateTime"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };

            return CreateJwtToken(claims, session.DateStart, session.DateEnd);
            
        }

        public async Task<string> CreateEmailToken(Guid userid)
        {
            User? user = await _userRepository.GetAsync(userid);
            if(user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            if(string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException(ResponseErrors.USER_EMAIL_NOT_SET);
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString(), "Guid"),
                new Claim("Email", user.Email)
            };

            return CreateJwtToken(claims, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(_emailLinkExpires));
        }

        private string CreateJwtToken(IEnumerable<Claim> claims, DateTime notBefore, DateTime expires)
        {
            var jwt = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    notBefore: notBefore,
                    claims: claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}

using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
        private readonly IConfirmationCodeRepository _codeRepository;

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _tokenExpires;

        private readonly string _email;
        private readonly string _password;
        private readonly string _name;
        private readonly string _server;
        private readonly int _port;
        private readonly int _emailExpires;

        public UserService(IOptions<EmailOptions> emailOptions,
            IOptions<JWTOptions> options, 
            IUserRepository userRepository, 
            IUserChatRepository userChatRepository,
            ISessionRepository sessionRepository,
            IConfirmationCodeRepository codeRepository,
            IServiceContext serviceContext)
        {
            _userRepository = userRepository;
            _userChatRepository = userChatRepository;
            _serviceContext = serviceContext;
            _sessionRepository = sessionRepository;
            _codeRepository = codeRepository;

            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _key = options.Value.Key;
            _tokenExpires = options.Value.Expires;

            _email = emailOptions.Value.Email;
            _password = emailOptions.Value.Password;
            _name = emailOptions.Value.Name;
            _server = emailOptions.Value.SmtpServer;
            _port = emailOptions.Value.Port;
            _emailExpires = emailOptions.Value.Expires;
        }

        public int EmailCodeExpires { get => _emailExpires; }
        public int TokenExpires { get => _tokenExpires; }

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
            if (id != _serviceContext.UserId)
            {
                IEnumerable<Guid> firstUserChats = await _userChatRepository.GetUserChatsAsync(id);
                IEnumerable<Guid> secondUserChats = await _userChatRepository.GetUserChatsAsync(_serviceContext.UserId);

                if (!firstUserChats.Intersect(secondUserChats).Any())
                {
                    throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
                }
            }

            return await _userRepository.GetAsync(id);
        }

        public async Task UpdateUser()
        {

        }

        public async Task DeleteUser(string reason)
        {
            await _userRepository.DeleteAsync(_serviceContext.UserId, reason);
        }

        public async Task ChangePassword(string password)
        {
            string hasedPassword = Password.GetHasedPassword(password);
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if (user.Password == hasedPassword)
            {
                throw new ArgumentException(ResponseErrors.PASSWORD_ALREADY_SET);
            }
            //await _userRepository.UpdateAsync(hasedPassword);
        }

        public async Task UpdateStatus(string status)
        {
            //await _userRepository.UpdateAsync(status);
        }

        public async Task<bool> ConfirmEmail(string code)
        {
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if (!user.IsConfirmed)
            {
                if(await CheckCodeAsync(code))
                {
                    //_userRepository.UpdateAsync(user.Id, true);
                }

                // Отправку нужно делать при изменинии профиля
                //SendToEmail("Подтверждение email", "");
            }

            return false;
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

        public async Task<string> SignIn(string phonenumber, string password, string deviceName)
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
                DateEnd = DateTime.UtcNow.AddSeconds(_tokenExpires)
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

            return encodedJwt;    
        }

        public async Task SignOut()
        {
            await _sessionRepository.UpdateAsync(_serviceContext.SessionId, DateTime.UtcNow);
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

        private List<Claim> GetClaimsForEmail(User user)
        {
            return new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString(), "Guid"),
                new Claim("Email", user.Email, "string"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };
        }

        public async Task<bool> CheckCodeAsync(string code)
        {
            ConfirmationCode confirmationCode = await _codeRepository.GetUnsedCodeByUser(_serviceContext.UserId);
            if (confirmationCode == null)
            {
                return false;
            }

            try
            {
                Password.VerifyHashedPassword(confirmationCode.Code, code);
                await _codeRepository.UpdateAsync(confirmationCode.Id, true);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task SendToEmailAsync(string subject, string content)
        {
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if(string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException(ResponseErrors.USER_EMAIL_NOT_SET);
            }

            MailAddress from = new MailAddress(_email, _name);
            MailAddress to = new MailAddress(user.Email);

            MailMessage m = new MailMessage(from, to)
            {
                Subject = subject,
                Body = content
            };

            SmtpClient smtp = new SmtpClient(_server, _port)
            {
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(m);
        }

        public async Task SendCodeAsync()
        {
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if (!user.IsConfirmed)
            {
                throw new InvalidOperationException(ResponseErrors.USER_HAS_UNCONFIRMED_EMAIL);
            }

            if (await _codeRepository.GetUnsedCodeByUser(user.Id) == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_HAS_CODE);
            }

            string hashedCode;
            do
            {
                hashedCode = Password.GetHasedPassword(GenerateCode());
            }
            while (await _codeRepository.UnUsedCodeExists(hashedCode));

            await _codeRepository.CreateAsync(new ConfirmationCode
            {
                Code = hashedCode,
                UserId = user.Id
            });
        }

        public async Task ResendCodeAsync()
        {
            ConfirmationCode code = await _codeRepository.GetUnsedCodeByUser(_serviceContext.UserId);
            if (code == null)
                throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_CODE);
            //Изменить существующий код в бд на новый
            string newHashedCode;
            do
            {
                newHashedCode = Password.GetHasedPassword(GenerateCode());
            }
            while (await _codeRepository.UnUsedCodeExists(newHashedCode));

            await _codeRepository.UpdateAsync(code.Id, newHashedCode);
        }

        private string GenerateCode()
        {
            string code = string.Empty;
            Random rnd = new Random();

            for (int i = 0; i < 6; i++)
                code += rnd.Next(0, 10).ToString();

            return code;
        }
    }
}

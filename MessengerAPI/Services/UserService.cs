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
        private readonly int _sessionExpires;
        private readonly int _emailLinkExpires;

        private readonly string _email;
        private readonly string _password;
        private readonly string _name;
        private readonly string _server;
        private readonly int _port;

        public UserService(IOptions<EmailOptions> emailOptions,
            IOptions<JwtOptions> options, 
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
            _sessionExpires = options.Value.SessionExpires;
            _emailLinkExpires = options.Value.EmailLinkExpires;

            _email = emailOptions.Value.Email;
            _password = emailOptions.Value.Password;
            _name = emailOptions.Value.Name;
            _server = emailOptions.Value.SmtpServer;
            _port = emailOptions.Value.Port;
        }

        public int EmailCodeExpires { get => _emailLinkExpires; }
        public int SessionExpires { get => _sessionExpires; }

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
<<<<<<< HEAD
                throw new ArgumentException(ResponseErrors.PASSWORD_ALREADY_SET);
=======
                //await _userRepository.UpdateAsync(hasedPassword);
>>>>>>> fideralRule
            }
            //await _userRepository.UpdateAsync(hasedPassword);
        }

        public async Task UpdateStatus(string status)
        {
            //await _userRepository.UpdateAsync(status);
<<<<<<< HEAD
=======
        }

        public async Task ConfirmEmail()
        {
            User user = await _userRepository.GetAsync(_serviceContext.UserId);
            if(!user.IsConfirmed)
            {

            }
>>>>>>> fideralRule
        }

        public async Task SignUp(User user, string password)
        {
            if (await _userRepository.FindByPhonenumberAsync(user.Phonenumber) != null ||
                await _userRepository.FindByNicknameAsync(user.Nickname) != null)
            {
                throw new ArgumentException(ResponseErrors.ALREADY_EXISTS);
            }
            if(!string.IsNullOrWhiteSpace(user.Email) && await _userRepository.FindByConfirmedEmailAsync(user.Email) != null)
            {
                throw new ArgumentException(ResponseErrors.EMAIL_ALREADY_EXIST);
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
                DateEnd = DateTime.UtcNow.AddSeconds(_sessionExpires)
            };
            await _sessionRepository.CreateAsync(session);

            return await CreateSessionToken(session);    
        }

        private async Task<string> CreateSessionToken(Session session)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, session.Id.ToString(), "Guid"),
                new Claim("DateEnd", session.DateEnd.ToString(), "DateTime"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
            };

            return CreateJwtToken(claims, session.DateStart, session.DateEnd);

        }

        public async Task<string> CreateEmailToken()
        {
            User? user = await _userRepository.GetAsync(_serviceContext.UserId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            if (string.IsNullOrWhiteSpace(user.Email))
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

        public async Task<bool> ConfirmEmail(string emailToken)
        {
            if (string.IsNullOrWhiteSpace(emailToken))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(emailToken);
            var tokenS = jsonToken as JwtSecurityToken;

            Guid userId = Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value);
            string email = tokenS.Claims.First(claim => claim.Type == "Email").Value;

            User user = await _userRepository.GetAsync(userId);
            if (user.Email != email)
            {
                return false;
            }
            if(!user.IsConfirmed)
            {
                //_userRepository.Update(true);
            }
            
            return true;
        }

        public async Task SignOut()
        {
            await _sessionRepository.UpdateAsync(_serviceContext.SessionId, DateTime.UtcNow);
        }

<<<<<<< HEAD
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
            {
                throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_CODE);
            }

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
=======
        public Task<bool> CheckCode(string code)
        {
            throw new NotImplementedException();
>>>>>>> fideralRule
        }
    }
}

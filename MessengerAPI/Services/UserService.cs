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
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            return user;
        }

        public async Task<User?> GetCurrentUser()
        {
            return await _userRepository.GetAsync(_serviceContext.UserId);
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

        public async Task UpdateUserInfo(string name, string surname, string nickname, string email)
        {
            User? user = await _userRepository.GetAsync(_serviceContext.UserId);
            await _userRepository.UpdateAsync(_serviceContext.UserId, "name", name);
            await _userRepository.UpdateAsync(_serviceContext.UserId, "surname", surname);
            await _userRepository.UpdateAsync(_serviceContext.UserId, "nickname", nickname);

            if (email != user.Email)
            {
                await _userRepository.UpdateAsync(_serviceContext.UserId, "email", email);
                await _userRepository.UpdateAsync(_serviceContext.UserId, "isconfirmed", false);
            }
        }

        public async Task DeleteUser(string reason)
        {
            await _userRepository.DeleteAsync(_serviceContext.UserId, reason);
        }

        public async Task ChangePassword(Guid? userid, string password)
        {
            if (userid.HasValue)
            {
                if (await _userRepository.GetAsync(userid.Value) == null)
                {
                    throw new ArgumentNullException(ResponseErrors.USER_NOT_FOUND);
                }
            }
            else
            {
                userid = _serviceContext.UserId;
            }

            if (password.Length < 8)
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            string hasedPassword = Password.GetHasedPassword(password);
            User user = await _userRepository.GetAsync(userid.Value);
            if (user.Password == hasedPassword)
            {
                throw new ArgumentException(ResponseErrors.PASSWORD_ALREADY_SET);
            }
            await _userRepository.UpdateAsync(userid.Value, "password", hasedPassword);
        }

        public async Task UpdateStatus(string status)
        {
            await _userRepository.UpdateAsync(_serviceContext.UserId, "status", status);
        }

        public async Task SignUp(User user, string password)
        {
            if (await _userRepository.FindByPhonenumberAsync(user.Phonenumber) != null ||
                (await _userRepository.FindByNicknameAsync(user.Nickname)).Count() != 0)
            {
                throw new ArgumentException(ResponseErrors.ALREADY_EXISTS);
            }
            if (!string.IsNullOrWhiteSpace(user.Email) && await _userRepository.FindByConfirmedEmailAsync(user.Email) != null)
            {
                throw new ArgumentException(ResponseErrors.EMAIL_ALREADY_EXIST);
            }

            user.Password = Password.GetHasedPassword(password);
            await _userRepository.CreateAsync(_userRepository.EntityToDictionary(user));
        }

        public async Task<Session> SignIn(string phonenumber, string password, string deviceName)
        {
            User? user = await _userRepository.FindByPhonenumberAsync(phonenumber);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            if (await _sessionRepository.GetUnfinishedOnDeviceAsync(deviceName, DateTime.UtcNow.ToLocalTime()) != null)
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

            await _sessionRepository.CreateAsync(_sessionRepository.EntityToDictionary(session));

            return session;
        }

        public async Task SignOut()
        {
            await _sessionRepository.UpdateAsync(_serviceContext.SessionId, "dateend", DateTime.UtcNow);
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
            if (!user.IsConfirmed)
            {
                await _userRepository.UpdateAsync(userId, "isconfirmed", true);
            }

            return true;
        }

        public async Task<ConfirmationCode> TryGetCodeInfoAsync(string code)
        {
            IEnumerable<ConfirmationCode> confirmationCodes = await _codeRepository.GetUnusedValidCode();
            if (confirmationCodes.Count() == 0)
            {
                throw new ArgumentNullException(ResponseErrors.INVALID_CODE);
            }

            foreach (ConfirmationCode codeHash in confirmationCodes)
            {
                try
                {
                    Password.VerifyHashedPassword(codeHash.Code, code);
                    await _codeRepository.UpdateAsync(codeHash.Id, "isused", true);

                    return codeHash;
                }
                catch (UnauthorizedAccessException) { }
            }

            throw new ArgumentNullException(ResponseErrors.INVALID_CODE);
        }

        public async Task SendToEmailAsync(string email, string subject, string content)
        {
            MailAddress from = new MailAddress(_email, _name);
            MailAddress to = new MailAddress(email);

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

        public async Task SendCodeAsync(string email)
        {
            User? user = await _userRepository.FindByConfirmedEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            if (await _codeRepository.GetUnsedCodeByUser(user.Id) != null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_HAS_CODE);
            }

            string hashedCode;
            string generatedCode;
            do
            {
                generatedCode = GenerateCode();
                hashedCode = Password.GetHasedPassword(generatedCode);
            }
            while (await _codeRepository.UnUsedCodeExists(hashedCode));

            ConfirmationCode code = new ConfirmationCode
            {
                Code = hashedCode,
                UserId = user.Id,
                DateStart = DateTime.UtcNow
            };

            await _codeRepository.CreateAsync(_codeRepository.EntityToDictionary(code));
            await SendToEmailAsync(user.Email, "Код восстановления", generatedCode);
        }

        public async Task ResendCodeAsync(string email)
        {
            User? user = await _userRepository.FindByConfirmedEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            ConfirmationCode code = await _codeRepository.GetUnsedCodeByUser(user.Id);
            if (code == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_CODE);
            }

            string newHashedCode;
            string generatedCode;
            do
            {
                generatedCode = GenerateCode();
                newHashedCode = Password.GetHasedPassword(generatedCode);
            }
            while (await _codeRepository.UnUsedCodeExists(newHashedCode));

            await _codeRepository.UpdateAsync(code.Id, "code", newHashedCode);
            await SendToEmailAsync(user.Email, "Код восстановления", generatedCode);
        }

        private string GenerateCode()
        {
            string code = string.Empty;
            Random rnd = new Random();

            for (int i = 0; i < 6; i++)
            {
                code += rnd.Next(0, 10).ToString();
            }

            return code;
        }
    }
}

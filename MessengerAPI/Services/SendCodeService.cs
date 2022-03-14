using MessengerAPI.Repositories;
using System.Net.Mail;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;
using System.Net;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.DTOs;

namespace MessengerAPI.Services
{
    public class SendCodeService : ISendCodeService
    {
        private readonly IConfirmationCodeRepository _codeRepository;
        private readonly IUserRepository _userRepository;
        private readonly string _email;
        private readonly string _password;
        private readonly string _name;
        private readonly string _server;
        private readonly int _port;
        private readonly int _expiries;

        public SendCodeService(IConfirmationCodeRepository codeRepository, IUserRepository userRepository, IOptions<EmailOptions> options)
        {
            _codeRepository = codeRepository;
            _userRepository = userRepository;
            _email = options.Value.Email;
            _password = options.Value.Password;
            _name = options.Value.Name;
            _server = options.Value.SmtpServer;
            _port = options.Value.Port;
            _expiries = options.Value.Expires;
        }

        public async Task<SendCodeResponse> SendCodeAsync(Guid userId)
        {
            if (await _codeRepository.UserHasUnUsedCode(userId))
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_HAS_CODE);

            User user = await _userRepository.GetAsync(userId);
            if (!user.IsConfirmed)
                throw new InvalidOperationException(ResponseErrors.USER_HAS_UNCONFIRMED_EMAIL);

            string code;
            do
            {
                code = GenerateCode();
            }
            while (await _codeRepository.UnUsedCodeExists(code));

            await _codeRepository.CreateAsync(new ConfirmationCode { Code = code, UserId=userId });

            MailAddress from = new MailAddress(_email, _name);
            MailAddress to = new MailAddress(user.Email);

            MailMessage m = new MailMessage(from, to)
            {
                Subject = "Код подтверждения",
                Body = "Вот код для подтверждения операции: " + code
            };

            SmtpClient smtp = new SmtpClient(_server, _port)
            {
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(m);

            return new SendCodeResponse { EmailCodeHash = code, Timeout = _expiries };
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

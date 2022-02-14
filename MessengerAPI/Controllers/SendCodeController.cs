using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace MessengerAPI.Controllers
{
    [Route("api/sendCode")]
    [ApiController]
    public class SendCodeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register()
        {
            MailAddress from = new MailAddress("somemail@gmail.com", "Tom");
            MailAddress to = new MailAddress("somemail@yandex.ru");
            MailMessage m = new MailMessage(from, to)
            {
                Subject = "Код подтверждения",
                Body = "Вот код для подтверждения операции: "
            };
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("somemail@gmail.com", "mypassword"),
                EnableSsl = true
            };
            await smtp.SendMailAsync(m);
            return Ok();
        }
    }
}

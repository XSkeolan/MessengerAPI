using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Npgsql;

namespace MessengerAPI.Controllers
{
    [Route("api/sendCode")]
    [ApiController]
    public class SendCodeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SendCodeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendCode(Guid userId)
        {
            using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();

                string code = string.Empty;
                string email = string.Empty;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT email, isconfirmed FROM Users WHERE id=@userid";

                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userid", userId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                            return BadRequest("User not found");
                        reader.Read();
                        if (!(bool)reader[1])
                            return BadRequest("Email is not confirmed");
                        email = (string)reader[0];
                    }
                }

                try
                {
                    MailAddress from = new MailAddress(_configuration["EmailConfiguration:Email"], "Tom");
                    MailAddress to = new MailAddress(email);

                    for (int i = 0; i < 6; i++)
                        code += new Random().Next(0, 10).ToString();

                    MailMessage m = new MailMessage(from, to)
                    {
                        Subject = "Код подтверждения",
                        Body = "Вот код для подтверждения операции: " + code
                    };

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_configuration["EmailConfiguration:Email"], _configuration["EmailConfiguration:Password"]),
                        EnableSsl = true
                    };

                    await smtp.SendMailAsync(m);
                }
                catch (Exception)
                {
                    return BadRequest("The code cannot be sent.No email specified");
                }

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO confirmationcode(code, userId) VALUES(@code, @userId)";

                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));
                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userId", userId));

                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }
    }
}

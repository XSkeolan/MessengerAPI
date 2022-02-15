using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Npgsql;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> SendCode(string email)
        {
            if(!Regex.IsMatch(email, "\\w+([\\.-]?\\w+)*@\\w+([\\.-]?\\w+)*\\.\\w{2,4}"))
                return BadRequest("Input parameter is not email");

            using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();

                string code = string.Empty;
                Guid userId;
                long count;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id,isconfirmed FROM Users WHERE email=@email";

                    command.Parameters.Add(new NpgsqlParameter<string>("@email", email));

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        if(reader.HasRows)
                        {
                            reader.Read();
                            if(!(bool)reader[1])
                            {
                                return BadRequest("Email is not confirmed");
                            }
                            else
                            {
                                userId = (Guid)reader[0];
                            }
                        }
                        else
                        {
                            return BadRequest("Email not found");
                        }
                    }
                }

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Count(*) FROM confirmationcode WHERE userid=@userId and isused=@isUsed";

                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userId", userId));
                    command.Parameters.Add(new NpgsqlParameter<bool>("@isUsed", false));

                    count = (long)(await command.ExecuteScalarAsync() ?? 0);
                }

                if(count!=0)
                {
                    return BadRequest("You do not used last code! If you want refresh code, use /resendCode api url");
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) FROM confirmationcode WHERE code=@code and isused=@isUsed";

                    for (int i = 0; i < 6; i++)
                        code += new Random().Next(0, 10).ToString();

                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));
                    command.Parameters.Add(new NpgsqlParameter<bool>("@isUsed", false));

                    while ((long)(await command.ExecuteScalarAsync() ?? 0) != 0)
                    {
                        code = string.Empty;
                        for (int i = 0; i < 6; i++)
                            code += new Random().Next(0, 10).ToString();

                        command.Parameters["@code"].Value = code;
                    }
                }

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

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO confirmationcode(code, userId) VALUES(@code, @userId)";

                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));
                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userId", userId));

                    await command.ExecuteNonQueryAsync();
                }

                connection.Close();
            }
            //information about code maybe
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MessengerAPI.Controllers
{
    [Route("api/resendCode")]
    [ApiController]
    public class ResendCodeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ResendCodeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> ResendCode(string email)
        {
            if (!Regex.IsMatch(email, "\\w+([\\.-]?\\w+)*@\\w+([\\.-]?\\w+)*\\.\\w{2,4}"))
                return BadRequest("Input parameter is not email");

            using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();

                Guid userId;
                Guid codeId;
                string code = string.Empty;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id,isconfirmed FROM Users WHERE email=@email";

                    command.Parameters.Add(new NpgsqlParameter<string>("@email", email));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (!(bool)reader[1])
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

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id FROM confirmationcode WHERE userid=@userId and isused=@isUsed";

                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userId", userId));
                    command.Parameters.Add(new NpgsqlParameter<bool>("@isUsed", false));

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            codeId = (Guid)reader[0];
                        }
                        else
                        {
                            return NoContent();
                        }
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) FROM confirmationcode WHERE code=@code and isused=@isUsed";

                    for (int i = 0; i < 6; i++)
                        code += new Random().Next(0, 10).ToString();

                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));
                    command.Parameters.Add(new NpgsqlParameter<bool>("@isUsed", false));

                    while((long)(await command.ExecuteScalarAsync() ?? 0) != 0)
                    {
                        code = string.Empty;
                        for (int i = 0; i < 6; i++)
                            code += new Random().Next(0, 10).ToString();

                        command.Parameters["@code"].Value = code;
                    }
                }

                MailAddress from = new MailAddress(_configuration["EmailConfiguration:Email"], "Tom");
                MailAddress to = new MailAddress(email);

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
                    command.CommandText = "UPDATE confirmationcode SET code=@code WHERE id=@codeId";

                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));
                    command.Parameters.Add(new NpgsqlParameter<Guid>("@codeId", codeId));

                    await command.ExecuteNonQueryAsync();
                }
            }
            //information about code maybe
            return Ok();
        }
    }
}

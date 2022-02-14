using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using MessengerAPI.Models;

namespace MessengerAPI.Controllers
{
    [Route("api/signIn")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SignInController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Sign(string phoneNumber, string password)
        {
            Guid sessionGuid;

            using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();

                Guid user;

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT id, password FROM Users WHERE PhoneNumber=@phonenumber";
                    command.Parameters.Add(new NpgsqlParameter<string>("@phonenumber", phoneNumber));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                            return BadRequest("Phone number not found! Register in messager");
                        else
                        {
                            reader.Read();
                            user = (Guid)reader[0];
                            if ((string)reader[1] != password)
                                return BadRequest("Password is not correct");
                        }
                    }
                }

                using(var command = connection.CreateCommand())
                {
                    sessionGuid = Guid.NewGuid();

                    command.CommandText = "INSERT INTO Sessions VALUES(@id, @datetime, @userId, @device)";
                    command.Parameters.Add(new NpgsqlParameter<Guid>("@id", sessionGuid));
                    command.Parameters.Add(new NpgsqlParameter<DateTime>("@datetime", DateTime.Now));
                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userId", user));
                    command.Parameters.Add(new NpgsqlParameter<string>(@"device", Request.Headers.UserAgent));

                    await command.ExecuteNonQueryAsync();
                }

                connection.Close();
            }
            // возможно лучше пользователя вернуть
            return Ok(sessionGuid);
        }
    }
}

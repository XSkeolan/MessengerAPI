using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Npgsql;

namespace MessengerAPI.Controllers
{
    [Route("api/recoverPassword")]
    [ApiController]
    public class RecoverPasswordController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RecoverPasswordController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(string code, string new_password)
        {
            // check condition
            if (!Regex.IsMatch(code, "\\d{6}") || code.Length != 6)
                return BadRequest("Code is not correctly");

            if (new_password.Length < 8 || new_password.Length > 32)
                return BadRequest("Password length should be акщь 10 to 32 characters");

            using(var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();

                Guid userId;
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT userId FROM confirmationcode WHERE code=@code and isused=@isUsed";

                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));
                    command.Parameters.Add(new NpgsqlParameter<bool>("@isUsed", false));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                            return BadRequest("Code not found or already used");

                        reader.Read();
                        userId = (Guid)reader[0];
                    }
                }

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE confirmationcode SET isused=@isUsed WHERE code=@code";

                    command.Parameters.Add(new NpgsqlParameter<bool>("@isUsed", true));
                    command.Parameters.Add(new NpgsqlParameter<string>("@code", code));

                    await command.ExecuteNonQueryAsync();
                }

                using(var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE users SET password=@newPassword WHERE id=@userId";

                    command.Parameters.Add(new NpgsqlParameter<string>("@newPassword", new_password));
                    command.Parameters.Add(new NpgsqlParameter<Guid>("@userId", userId));

                    await command.ExecuteNonQueryAsync();
                }
            }
            // may be need to create session and return user
            return Ok();
        }
    }
}

using MessengerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text.RegularExpressions;

namespace MessengerAPI.Controllers
{
    [Route("api/singUp")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Register(string phoneNumber, string name, string surname, string password, string nickName = "")
        {
            if (!Regex.IsMatch(phoneNumber, @"\d{11}") || phoneNumber.Length!=11)
                return BadRequest("Phone number has an incorrect format");

            if (name.Length > 50)
                return BadRequest("Name is very long");
            if (surname.Length > 50)
                return BadRequest("Surname is very long");
            if (password.Length > 32 || password.Length<10)
                return BadRequest("Password length must be from 10 to 32 chars");

            long count;
            User user;

            using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT count(*) FROM Users WHERE phonenumber=@phonenumber";
                    command.Parameters.Add(new NpgsqlParameter<string>("@phonenumber", phoneNumber));

                    count = (long)(await command.ExecuteScalarAsync() ?? 0);
                }

                if (count > 0)
                    return BadRequest("This phonenumber already exists");

                user = new User { Id = Guid.NewGuid(), Name = name, PhoneNumber = phoneNumber, Surname = surname };

                if (nickName == string.Empty)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT COUNT(*) FROM User";
                        count = (long)(await command.ExecuteScalarAsync() ?? 0);
                        user.NickName = ("UnnamedUser" + count)[..20]; // можно рандомом заменить
                    }
                }
                else
                {
                    user.NickName = nickName;
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Users VALUES(@id, @nickName, @password, @phonenumber, @name, @surname)";

                    command.Parameters.Add(new NpgsqlParameter<Guid>("@id", user.Id));
                    command.Parameters.Add(new NpgsqlParameter<string>("@nickName", user.NickName));
                    command.Parameters.Add(new NpgsqlParameter<string>("@phonenumber", phoneNumber));
                    command.Parameters.Add(new NpgsqlParameter<string>("@password", password));
                    command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
                    command.Parameters.Add(new NpgsqlParameter<string>("@surname", surname));

                    await command.ExecuteNonQueryAsync();
                }

                connection.Close();
            }
            return Ok(user);
        }
    }
}

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
            if (phoneNumber == null || password == null)
                return BadRequest("Some field are filled in");
            Guid guid;
            using (var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();
                Guid user;
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT id, phonenumber, password FROM Users WHERE PhoneNumber='{phoneNumber}'";
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if(!reader.HasRows)
                        return BadRequest("Phone number not found! Register in message");
                    else
                    {
                        reader.Read();
                        user = (Guid)reader[0];
                        Console.WriteLine((string)reader[0], (string)reader[1], (string)reader[2]);
                        
                        if ((string)reader[2] != password)
                            return BadRequest("Password is not correct");
                    }
                }
                using(var command = connection.CreateCommand())
                {
                    guid = Guid.NewGuid();
                    command.CommandText = $"INSERT INTO Session VALUES({guid}, {DateTime.Now}, {user}, {Request.Headers.UserAgent})";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return Ok(guid);
        }
    }
}

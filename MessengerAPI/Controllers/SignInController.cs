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

            using(var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]))
            {
                connection.Open();
                using(var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT phonenumber,password FROM Users WHERE PhoneNumber='{phoneNumber}'";
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if(!reader.HasRows)
                        return BadRequest("Phone number not found");
                    else
                    {
                        reader.Read();
                        Console.WriteLine((string)reader[0], (string)reader[1]);
                        if ((string)reader[1] == password)
                        {
                            //Session session = new Session();
                            return Ok();
                        }
                        else
                            return BadRequest("Password is not correct");
                    }
                }
                connection.Close();
            }
        }
    }
}

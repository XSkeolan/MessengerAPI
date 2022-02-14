using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MessengerAPI.Controllers
{
    [Route("api/signOut")]
    [ApiController]
    public class SignOutController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SignOutController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SignOut(Guid sessionId)
        {
            using var connection = new NpgsqlConnection(_configuration["ConnectionStrings:MessengerAPI"]);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Sessions SET dateEnd=@date WHERE id=@sessionId";

            command.Parameters.Add(new NpgsqlParameter<DateTime>("@date", DateTime.Now));
            command.Parameters.Add(new NpgsqlParameter<Guid>("@sessionId", sessionId));
            command.ExecuteNonQuery();
            if (await command.ExecuteNonQueryAsync() != 0)
                return Ok();
            else
                return BadRequest("Session Not found");
        }
    }
}

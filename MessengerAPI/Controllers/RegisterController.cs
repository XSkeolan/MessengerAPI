using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/singUp")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public IActionResult Register(string phoneNumber, string name, string surname, string password)
        {
            if (phoneNumber == null || name == null || surname == null || password == null)
                return BadRequest("Some field are filled in");


            return BadRequest("This username already exists");
            return Ok("User registered");
        }
    }
}

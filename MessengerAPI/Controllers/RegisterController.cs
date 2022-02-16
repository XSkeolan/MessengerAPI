using MessengerAPI.Services;
using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace MessengerAPI.Controllers
{
    [Route("api/singUp")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ISignUpService _signUpService;
        public RegisterController(IConfiguration configuration)
        {
            _signUpService = new SignUpService(configuration);
        }

        [HttpPost]
        public IActionResult Register(SignUpRequest user)
        {
            if (!Regex.IsMatch(user.Phonenumber, @"\d{11}") || user.Phonenumber.Length != 11)
                return BadRequest("Phone number has an incorrect format");

            if (user.Name.Length > 50)
                return BadRequest("Name is very long");
            if (user.Surname.Length > 50)
                return BadRequest("Surname is very long");
            if (user.Password.Length > 32 || user.Password.Length < 10)
                return BadRequest("Password length must be from 10 to 32 chars");

            SignUpResponse response;
            try
            {
                response = _signUpService.SignUp(user, Request.Headers.UserAgent);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(response);
        }
    }
}

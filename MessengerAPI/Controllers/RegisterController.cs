using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace MessengerAPI.Controllers
{
    [Route("api/public")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ISignUpService _signUpService;

        public RegisterController(ISignUpService signUp)
        {
            _signUpService = signUp;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(SignUpRequestUser inputUser)
        {
            if (!Regex.IsMatch(inputUser.Phonenumber, @"\d{11}") || inputUser.Phonenumber.Length != 11)
            {
                return BadRequest(ResponseErrors.PHONENUMBER_INCORRECT);
            }

            if (inputUser.Name.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (inputUser.Surname.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (inputUser.Password.Length > 32 || inputUser.Password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }
            if(inputUser.Nickname.Length > 20)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            User user = new User
            {
                Phonenumber = inputUser.Phonenumber,
                Name = inputUser.Name,
                Surname = inputUser.Surname,
                Nickname = inputUser.Nickname
            };

            try
            {
                UserResponse signUp = await _signUpService.SignUp(user, Request.Headers.UserAgent, inputUser.Password);
                return Ok(signUp);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

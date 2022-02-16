using MessengerAPI.Services;
using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Repositories;

namespace MessengerAPI.Controllers
{
    [Route("api/singUp")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ISignUpService _signUpService;
        private readonly IUserRepository _users;

        public RegisterController(IConfiguration configuration)
        {
            _signUpService = new SignUpService(configuration);
            _users = new UserRepository(configuration.GetConnectionString("MessengesAPI"));
        }

        [HttpPost]
        public async Task<IActionResult> Register(SignUpRequestUser inputUser)
        {
            if (!Regex.IsMatch(inputUser.Phonenumber, @"\d{11}") || inputUser.Phonenumber.Length != 11)
            {
                return BadRequest(ResponseErrors.PHONENUMBER_INCORRECT);
            }

            if (inputUser.Name.Length > 50)
            {
                return BadRequest("Name is very long");
            }
            if (inputUser.Surname.Length > 50)
            {
                return BadRequest("Surname is very long");
            }
            if (inputUser.Password.Length > 32 || inputUser.Password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            User? user = await _users.FindByPhonenumberAsync(inputUser.Phonenumber);
            if (user != null)
            {
                return BadRequest(ResponseErrors.PHONENUMBER_ALREADY_EXISTS);
            }

            user = await _users.FindByNicknameAsync(inputUser.Nickname);
            if (user != null)
            {
                return BadRequest(ResponseErrors.NICKNAME_ALREADY_EXISTS);
            }

            user = new User
            {
                Password = inputUser.Password,
                Phonenumber = inputUser.Phonenumber,
                Name = inputUser.Name,
                Surname = inputUser.Surname,
                Nickname = inputUser.Nickname
            };

            return Ok(await _signUpService.SignUp(user, new Session() { DeviceName = Request.Headers.UserAgent, DateStart = DateTime.Now }));
        }
    }
}

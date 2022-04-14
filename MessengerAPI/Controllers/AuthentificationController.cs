using MessengerAPI.Services;
using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using MessengerAPI.Models;

namespace MessengerAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthentificationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            return Ok();
        }

        [HttpPatch("user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest request) 
        {
            return Ok();
        }

        [HttpPatch("updateStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromBody] string status)
        {
            return Ok();
        }

        [HttpDelete("user")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody] string reason)
        {
            return Ok();
        }

        [HttpPatch("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string newPassword)
        {
            if (newPassword.Length > 32 || newPassword.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }
            return Ok();
        }

        [HttpPatch("confirmEmail")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail(Guid id)
        {
            return Ok();
        }

        [HttpPost("signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInRequest inputUser)
        {
            if (!Regex.IsMatch(inputUser.Phonenumber, @"\d{11}") || inputUser.Phonenumber.Length != 11)
            {
                return BadRequest(ResponseErrors.PHONENUMBER_INCORRECT);
            }

            if (inputUser.Password.Length > 32 || inputUser.Password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            try
            {
                return Ok(await _userService.SignIn(inputUser.Phonenumber, inputUser.Password, Request.Headers.UserAgent));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("signUp")]
        [AllowAnonymous]
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
            if (inputUser.Nickname.Length > 20)
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
                await _userService.SignUp(user, inputUser.Password);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpHead("signOut")]
        [Authorize]
        public async new Task<IActionResult> SignOut()
        {
            try
            {
                await _userService.SignOut();
                return Ok();
            }
            catch
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(string code, string newPassword)
        {
            if (!Regex.IsMatch(code, @"^\d{6}$"))
            {
                return BadRequest(ResponseErrors.INVALID_CODE);
            }
            if (newPassword.Length > 32 || newPassword.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            if(!await _userService.CheckCode(code))
            {
                return BadRequest(ResponseErrors.INVALID_CODE);
            }
            await _userService.ChangePassword(newPassword);
            return Ok();
        }
    }
}

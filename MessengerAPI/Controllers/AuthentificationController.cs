using MessengerAPI.Interfaces;
using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILinkService _linkService;

        public AuthentificationController(IUserService userService, ILinkService linkService)
        {
            _userService = userService;
            _linkService = linkService;
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            User? user = await _userService.GetUser(id);
            if (user == null)
            {
                return BadRequest(ResponseErrors.USER_NOT_FOUND);
            }

            return Ok(new BaseUserResponse
            {
                Id = id,
                Name = user.Name,
                Surname = user.Surname,
                Nickname = user.Nickname
            });
        }

        [HttpPatch("user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) &&
                string.IsNullOrWhiteSpace(request.Surname) &&
                string.IsNullOrWhiteSpace(request.NickName) &&
                string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            await _userService.UpdateUser();

            return Ok();
        }

        [HttpPatch("updateStatus")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus([FromBody] string newStatus)
        {
            if (newStatus.Length > 255)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            await _userService.UpdateStatus(newStatus);

            return Ok();
        }

        [HttpDelete("user")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody] string reason)
        {
            if (reason.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            await _userService.DeleteUser(reason);

            return Ok();
        }

        [HttpPatch("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            if (newPassword.Length > 32 || newPassword.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            await _userService.ChangePassword(newPassword);

            return Ok();
        }

        [HttpPatch("sendLinkOnEmail")]
        [Authorize]
        public async Task<IActionResult> SendLink()
        {
            string link;
            try
            {
                link = await _linkService.GetEmailLink();
                await _userService.SendToEmailAsync("Подтверждение почты", "Мы рады, что вы используете наш сервис. Чтобы подтвердить ваш аккаунт, перейдите по ссылке\n" + link);

                return Ok();
            }
            catch(InvalidCastException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string emailHash)
        {
            if(string.IsNullOrWhiteSpace(emailHash))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            //разкодировать параметр
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
                return Ok(new SignInResponse
                {
                    Expiries = _userService.TokenExpires,
                    Token = await _userService.SignIn(inputUser.Phonenumber, inputUser.Password, Request.Headers.UserAgent)
                });
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

            if (!await _userService.CheckCodeAsync(code))
            {
                return BadRequest(ResponseErrors.INVALID_CODE);
            }

            await _userService.ChangePassword(newPassword);

            return Ok();
        }
    }
}

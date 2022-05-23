using MessengerAPI.Interfaces;
using MessengerAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using MessengerAPI.Models;
using System.Net.Mail;

namespace MessengerAPI.Controllers
{
    [Route("api/private/auth")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILinkService _linkService;
        private readonly ITokenService _tokenService;

        public AuthentificationController(IUserService userService, ILinkService linkService, ITokenService tokenService)
        {
            _userService = userService;
            _linkService = linkService;
            _tokenService = tokenService;
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                User? user = await _userService.GetUser(id);

                return Ok(new BaseUserResponse
                {
                    Id = id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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

            if (request.Name.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (request.Surname.Length > 50)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }
            if (request.NickName.Length > 20)
            {
                return BadRequest(ResponseErrors.FIELD_LENGTH_IS_LONG);
            }

            try
            {
                MailAddress m = new MailAddress(request.Email);
            }
            catch (FormatException)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            await _userService.UpdateUserInfo(request.Name, request.Surname, request.NickName, request.Email);

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

            await _userService.ChangePassword(null, newPassword);

            return Ok();
        }

        [HttpPatch("sendLinkOnEmail")]
        [Authorize]
        public async Task<IActionResult> SendLink()
        {
            string link;
            try
            {
                string emailToken = await _tokenService.CreateEmailToken();
                link = await _linkService.GetEmailLink(emailToken);
                await _userService.SendToEmailAsync((await _userService.GetCurrentUser()).Email,"Подтверждение почты", "Мы рады, что вы используете наш сервис. Чтобы подтвердить ваш аккаунт, перейдите по ссылке\n" + link);

                return Ok();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/api/public/auth/confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string emailToken)
        {
            if(string.IsNullOrWhiteSpace(emailToken))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            
            return Ok(await _userService.ConfirmEmail(emailToken));
        }

        [HttpPost("/api/public/auth/signIn")]
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
                Session session = await _userService.SignIn(inputUser.Phonenumber, inputUser.Password, Request.Headers.UserAgent);

                return Ok(new SignInResponse
                {
                    Expiries = _userService.SessionExpires,
                    Token = await _tokenService.CreateSessionToken(session.Id)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/public/auth/signUp")]
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

        [HttpPost("/api/public/auth/recover")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(string code, string newPassword)
        {
            if (code.Length < 6)
            {
                return BadRequest(ResponseErrors.INVALID_CODE);
            }
            if (newPassword.Length > 32 || newPassword.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            try
            {
                ConfirmationCode codeInfo = await _userService.TryGetCodeInfoAsync(code);
                await _userService.ChangePassword(codeInfo.UserId, newPassword);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("/api/public/auth/requestCode")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestCode([FromBody]string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                await _userService.SendCodeAsync(email);
                return Ok();
            }
            catch(FormatException)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/api/public/auth/resendCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendCode([FromBody] string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                await _userService.ResendCodeAsync(email);
                return Ok();
            }
            catch (FormatException)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

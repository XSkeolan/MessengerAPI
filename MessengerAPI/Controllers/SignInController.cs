using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;

namespace MessengerAPI.Controllers
{
    [Route("api/signIn")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly ISignInService _signInService;

        public SignInController(ISignInService signInService)
        {
            _signInService = signInService;
        }

        [HttpPost]
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
                return Ok(await _signInService.SignIn(inputUser.Phonenumber, inputUser.Password));
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

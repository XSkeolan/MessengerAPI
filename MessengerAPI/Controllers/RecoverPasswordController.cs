using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Npgsql;

namespace MessengerAPI.Controllers
{
    [Route("api/private/recoverPassword")]
    [ApiController]
    public class RecoverPasswordController : ControllerBase
    {
        public RecoverPasswordController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(string code, string new_password)
        {
            if (!Regex.IsMatch(code, @"^\d{6}$"))
            {
                return BadRequest(ResponseErrors.INVALID_CODE);
            }
            if (new_password.Length > 32 || new_password.Length < 10)
            {
                return BadRequest(ResponseErrors.INVALID_PASSWORD);
            }

            // may be need to create session and return user
            return Ok();
        }
    }
}

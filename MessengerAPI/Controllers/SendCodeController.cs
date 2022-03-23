using Microsoft.AspNetCore.Mvc;
using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MessengerAPI.Controllers
{
    [Route("api/private/sendCode")]
    [ApiController]
    public class SendCodeController : ControllerBase
    {
        private readonly ISendCodeService _sendCodeService;

        public SendCodeController(ISendCodeService sendCodeService)
        {
            _sendCodeService = sendCodeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SendCode(string email)
        {
            try
            {
                return Ok(await _sendCodeService.SendCodeAsync(email));
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

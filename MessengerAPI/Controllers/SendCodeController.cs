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
        public async Task<IActionResult> SendCode()
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Items["User"].ToString());
                return Ok(await _sendCodeService.SendCodeAsync(userId));
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

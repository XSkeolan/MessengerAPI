using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private")]
    [ApiController]
    public class SignOutController : ControllerBase
    {
        private readonly ISignOutService _signOutService;
        public SignOutController(ISignOutService signOutService)
        {
            _signOutService = signOutService;
        }

        [HttpPost]
        [Route("signOut")]
        [Authorize]
        public async new Task<IActionResult> SignOut()
        {
            try
            {
                Guid userId = Guid.Parse(HttpContext.Items["User"].ToString());
                Guid sessionId = Guid.Parse(HttpContext.Items["Session"].ToString());
                await _signOutService.SignOut(sessionId, userId);
                return Ok();
            }
            catch
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
        }
    }
}

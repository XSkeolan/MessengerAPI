using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/private/signOut")]
    [ApiController]
    public class SignOutController : ControllerBase
    {
        private readonly ISignOutService _signOutService;
        public SignOutController(ISignOutService signOutService)
        {
            _signOutService = signOutService;
        }

        [HttpPost]
        [Authorize]
        public async new Task<IActionResult> SignOut()
        {
            try
            {
                await _signOutService.SignOut();
                return Ok();
            }
            catch
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
        }
    }
}

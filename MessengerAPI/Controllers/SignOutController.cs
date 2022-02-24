using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/signOut")]
    [ApiController]
    public class SignOutController : ControllerBase
    {
        private readonly ISignOutService _signOutService;
        public SignOutController(ISignOutService signOutService)
        {
            _signOutService = signOutService;
        }

        [HttpPost]
        public async Task<IActionResult> SignOut(SignOutRequest signOutRequest)
        {
            try
            {
                await _signOutService.SignOut(signOutRequest.SessionId, signOutRequest.UserId);
                return Ok();
            }
            catch
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
        }
    }
}

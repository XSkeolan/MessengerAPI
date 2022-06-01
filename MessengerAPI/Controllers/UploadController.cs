using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("private/uploadChatImage")]
        [Authorize]
        public async Task<IActionResult> UploadChatImage(IFormFile imageData)
        {
            if (imageData.Length==0)
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            return Ok(await _fileService.UploadFile(imageData));
        }
    }
}

using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

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
        public async Task<IActionResult> UploadChatImage([FromBody] string imageData)
        {
            if (string.IsNullOrWhiteSpace(imageData))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }

            var imageDataByteArray = Convert.FromBase64String(imageData);
            return Ok(await _fileService.UploadFile(imageDataByteArray));
        }

        [HttpPost("public/remoteFile")]
        public async Task<IActionResult> SaveImage(byte[] array)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("D:\\Image");
            int count = directoryInfo.EnumerateFiles().Count() + 1;
            Image m = Image.FromStream(new MemoryStream(array));
            string filename = "image" + count;
            m.Save(filename);

            return Ok(filename);
        }
    }
}

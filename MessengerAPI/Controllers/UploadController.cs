using MessengerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> UploadChatImage([FromBody] string imageData)
        {
            var imageDataByteArray = Convert.FromBase64String(imageData);
            if (string.IsNullOrWhiteSpace(imageData))
            {
                return BadRequest(ResponseErrors.INVALID_FIELDS);
            }
            //return File(imageDataByteArray, "image/png");
            return Ok(await _fileService.UploadFile(imageDataByteArray));
        }

        [HttpPost("private/ifile")]
        [Authorize]
        public async Task<IActionResult> UploadAsIFile(IFormFile formFile)
        {
            //using (Stream s = formFile.OpenReadStream())
            //{
            //    byte[] buffer = new byte[16*1024];
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        int read;
            //        while ((read = ms.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            ms.Write(buffer, 0, read);
            //        }
            //        await _fileService.UploadFile(ms.ToArray());
            //    }
            //}
            using (MemoryStream ms = new MemoryStream())
            {
                await formFile.CopyToAsync(ms);
                await _fileService.UploadFile(ms.ToArray());
            }
            return Ok();
        }
        [HttpPost("public/remoteFile")]
        public async Task<IActionResult> TestPost(byte[] array)
        {
            Console.WriteLine("RemoteFile");
            return Ok(array.GetHashCode());
        }
    }
}

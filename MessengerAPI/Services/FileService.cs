using MessengerAPI.Interfaces;

namespace MessengerAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<Guid> UploadFile(byte[] byteFile)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5037/")
            };

            var response = await client.PostAsync("api/Upload/public/remoteFile", new ByteArrayContent(byteFile));
            string filename = await response.Content.ReadAsStringAsync();

            Models.File file = new Models.File
            {
                Server = "http://localhost:5037/",
                Path = filename
            };

            await _fileRepository.CreateAsync(_fileRepository.EntityToDictionary(file));
            return file.Id;
        }
    }
}

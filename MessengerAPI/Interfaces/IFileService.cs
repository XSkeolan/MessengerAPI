namespace MessengerAPI.Interfaces
{
    public interface IFileService
    {
        Task<Guid> UploadFile(IFormFile byteFile);
    }
}
namespace MessengerAPI.Interfaces
{
    public interface IFileService
    {
        Task<Guid> UploadFile(byte[] byteFile);
    }
}
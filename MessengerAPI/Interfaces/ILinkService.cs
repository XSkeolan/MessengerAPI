namespace MessengerAPI.Interfaces
{
    public interface ILinkService
    {
        Task<string> GetEmailLink(string emailToken);
    }
}
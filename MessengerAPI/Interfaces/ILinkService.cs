namespace MessengerAPI.Interfaces
{
    public interface ILinkService
    {
        Task<string> GetChannelLink();
        Task<string> GetEmailLink(string emailToken);
    }
}
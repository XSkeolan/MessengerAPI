namespace MessengerAPI.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateEmailToken();
        Task<string> CreateSessionToken(Guid sessionId);
        Task<string> CreateInvitationToken(Guid channelLinkId);
    }
}
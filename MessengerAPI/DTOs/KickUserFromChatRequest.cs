namespace MessengerAPI.DTOs
{
    public class KickUserFromChatRequest
    {
        public Guid ChatId { get; set; }
        public IEnumerable<KickUserRequest> KickUsers { get; set; }
    }
}

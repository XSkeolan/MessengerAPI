namespace MessengerAPI.DTOs
{
    public class InviteToChatRequest
    {
        public Guid ChatId { get; set; }
        public IEnumerable<Guid> InviteUsers { get; set; }
    }
}

namespace MessengerAPI.DTOs
{
    public class InviteUserRequest
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
    }
}

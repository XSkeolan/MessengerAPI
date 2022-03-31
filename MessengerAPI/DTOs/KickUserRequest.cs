namespace MessengerAPI.DTOs
{
    public class KickUserRequest
    {
        public Guid UserId { get; set; }
        public string? Reason { get; set; }
    }
}

namespace MessengerAPI.DTOs
{
    public class SignOutRequest
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
    }
}

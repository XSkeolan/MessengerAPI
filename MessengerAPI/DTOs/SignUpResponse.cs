namespace MessengerAPI.DTOs
{
    public class SignUpResponse
    {
        public Guid SessionId { get; set; }
        public UserResponse User { get; set; }
    }
}

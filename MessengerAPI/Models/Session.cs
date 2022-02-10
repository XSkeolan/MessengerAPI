namespace MessengerAPI.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public DateTime TimeStart { get; set; }
        public Guid UserId { get; set; }
        public string DeviceName { get; set; }
    }
}

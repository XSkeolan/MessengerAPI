namespace MessengerAPI.Models
{
    public class Session : EntityBase
    {
        public DateTime DateStart { get; set; }
        public Guid UserId { get; set; }
        public string DeviceName { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}

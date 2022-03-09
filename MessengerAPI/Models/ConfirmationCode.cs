namespace MessengerAPI.Models
{
    public class ConfirmationCode
    {
        public string Code { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateEnd { get; set; }
    }
}

namespace MessengerAPI.Models
{
    public class ConfirmationCode : EntityBase
    {
        public string Code { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsUsed { get; set; }
    }
}

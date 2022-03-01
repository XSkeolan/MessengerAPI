namespace MessengerAPI.Models
{
    public class UserChat:EntityBase
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserTypeId  { get; set; }
    }
}

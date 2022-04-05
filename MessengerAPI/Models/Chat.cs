namespace MessengerAPI.Models
{
    public class Chat : EntityBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? PhotoId { get; set; }
        public DateTime Created { get; set; }
        public Guid CreatorId { get; set; }
    }
}

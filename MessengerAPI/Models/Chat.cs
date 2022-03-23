namespace MessengerAPI.Models
{
    public class Chat : EntityBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public InputFile? Photo { get; set; }
        public DateTime Created { get; set; }
    }
}

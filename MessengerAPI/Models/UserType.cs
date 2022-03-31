namespace MessengerAPI.Models
{
    public class UserType : EntityBase
    {
        public string TypeName { get; set; }
        public string Permissions { get; set; }
        public short PriorityLevel { get; set; }
        public bool IsDefault { get; set; }
    }
}

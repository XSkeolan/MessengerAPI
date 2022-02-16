namespace MessengerAPI.DTOs
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Phonenumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsConfirmed { get; set; }
    }
}

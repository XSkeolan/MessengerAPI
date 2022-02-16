namespace MessengerAPI.DTOs
{
    public class SignUpRequest
    {
        public string Phonenumber { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; } = "UnnamedUser" + new Random().Next(10000, 100000);
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}

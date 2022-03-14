namespace MessengerAPI.DTOs
{
    public class SendCodeResponse
    {
        public string EmailCodeHash { get; set; }
        public int Timeout { get; set; }
    }
}

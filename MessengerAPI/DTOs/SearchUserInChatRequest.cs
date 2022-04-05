namespace MessengerAPI.DTOs
{
    public class SearchUserInChatRequest
    {
        public Guid ChatId { get; set; }
        public string SubString { get; set; }
        public int LimitResult { get; set; }
    }
}

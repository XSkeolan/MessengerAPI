namespace MessengerAPI.DTOs
{
    public class HistoryResponse
    {
        public Guid ChatId { get; set; }
        public IEnumerable<Guid> MessagesId { get; set; }
    }
}

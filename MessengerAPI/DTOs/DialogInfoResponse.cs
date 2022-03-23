namespace MessengerAPI.DTOs
{
    public class DialogInfoResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte Photo { get; set; }
        public string? LastMessageText { get; set; }
        public DateTime? LastMessageDateSend { get; set; }
    }
}

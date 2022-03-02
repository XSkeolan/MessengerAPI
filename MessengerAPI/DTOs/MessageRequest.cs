using System.Text.Json.Serialization;

namespace MessengerAPI.DTOs
{
    public class MessageRequest
    {
        public string Message { get; set; }
        public Guid From { get; set; }
        public Guid To { get; set; }
        public Guid? ReplyMessageId { get; set; }
    }
}

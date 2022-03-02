using MessengerAPI.Models;
using System.Text.Json.Serialization;

namespace MessengerAPI.DTOs
{
    public class MessageRequest
    {
        public string Message { get; set; }
        public Guid From { get; set; }
        public Guid Destination { get; set; }
        public DestinationType DestinationType { get; set; }
        public Guid? ReplyMessageId { get; set; }
    }
}

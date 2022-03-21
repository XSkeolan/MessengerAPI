﻿using MessengerAPI.Models;

namespace MessengerAPI.DTOs
{
    public class MessageResponse
    {
        public Guid MessageId { get; set; }
        public Guid FromId { get; set; }
        public Guid DestinationId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
    }
}

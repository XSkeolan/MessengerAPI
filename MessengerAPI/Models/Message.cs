﻿using System.Text.Json.Serialization;

namespace MessengerAPI.Models
{
    public class Message : EntityBase
    {
        public string Text { get; set; }
        public DateTime DateSend { get; set; }
        public Guid From { get; set; }
        public Guid Destination { get; set; }
        public bool IsPinned { get; set; }
        [JsonExtensionData]
        public Guid? OriginalMessageId { get; set; }
    }
}

﻿namespace MessengerAPI.DTOs
{
    public class KickUserRequest
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public string? Reason { get; set; }
    }
}

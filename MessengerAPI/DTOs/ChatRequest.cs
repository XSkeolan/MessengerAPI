﻿namespace MessengerAPI.DTOs
{
    public class ChatRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Photo { get; set; }
        public Guid AdministratorId { get; set; }
    }
}
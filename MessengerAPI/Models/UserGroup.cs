﻿namespace MessengerAPI.Models
{
    public class UserGroup:EntityBase
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserTypeId  { get; set; }
    }
}
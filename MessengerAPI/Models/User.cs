﻿namespace MessengerAPI.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Email { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDeleted { get; set; }
    }
}

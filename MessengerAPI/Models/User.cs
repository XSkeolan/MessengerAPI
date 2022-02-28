namespace MessengerAPI.Models
{
    public class User : EntityBase
    {
        public string Nickname { get; set; }
        /// <summary>
        /// Пароль в виде хеша
        /// </summary>
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Email { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDeleted { get; set; }
    }
}

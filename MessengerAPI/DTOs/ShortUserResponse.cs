namespace MessengerAPI.DTOs
{
    public class ShortUserResponse
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Ник пользователя
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Тип пользователя в чате
        /// </summary>
        public Guid UserTypeId { get; set; }
    }
}
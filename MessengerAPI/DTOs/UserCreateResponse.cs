namespace MessengerAPI.DTOs
{
    public class UserResponse
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Ник пользователя
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        public string Phonenumber { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Подтвержден ли email
        /// </summary>
        public bool IsConfirmed { get; set; }
    }
}

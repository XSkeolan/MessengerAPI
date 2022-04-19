namespace MessengerAPI.Models
{
    public class User : EntityBase
    {
        /// <summary>
        /// Ник пользователя
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// Пароль в виде хеша
        /// </summary>
        public string Password { get; set; }
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
        /// Электронная почта пользователя
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Является ли почта подтвержденной
        /// </summary>
        public bool IsConfirmed { get; set; } = false;
        /// <summary>
        /// Статус пользователя
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Причина удаления аккаунта
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
}

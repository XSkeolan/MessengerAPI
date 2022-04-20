namespace MessengerAPI.Models
{
    public class Session : EntityBase
    {
        /// <summary>
        /// Дата начала сессии
        /// </summary>
        public DateTime DateStart { get; set; }
        /// <summary>
        /// Идентификатор пользователя который начал сессию
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Устройство с которого был выполнен вход
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// Дата окончания сессии
        /// </summary>
        public DateTime DateEnd { get; set; }
    }
}

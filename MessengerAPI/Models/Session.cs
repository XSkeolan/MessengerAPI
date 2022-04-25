using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    public class Session : EntityBase
    {
        /// <summary>
        /// Дата начала сессии
        /// </summary>
        [Column("datestart")]
        public DateTime DateStart { get; set; }
        /// <summary>
        /// Идентификатор пользователя который начал сессию
        /// </summary>
        [Column("userid")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Устройство с которого был выполнен вход
        /// </summary>
        [Column("devicename")]
        public string DeviceName { get; set; }
        /// <summary>
        /// Дата окончания сессии
        /// </summary>
        [Column("dateend")]
        public DateTime DateEnd { get; set; }
    }
}

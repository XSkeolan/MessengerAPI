using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Models
{
    [Table("sessions")]
    public class Session : EntityBase
    {
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        /// <summary>
        /// Дата начала сессии
        /// </summary>
        [Column("datestart")]
        public DateTime DateStart { get => _startDateTime; set => _startDateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
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
        public DateTime DateEnd { get => _endDateTime; set => _endDateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }
}

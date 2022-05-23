using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("confirmationcode")]
    public class ConfirmationCode : EntityBase
    {
        private DateTime _startDatetime;
        /// <summary>
        /// Код для подтверждения
        /// </summary>
        [Column("code")]
        public string Code { get; set; }
        /// <summary>
        /// Идентификатор пользователя, к которому он привязан
        /// </summary>
        [Column("userid")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Дата и время окончания действия кода
        /// </summary>
        [Column("datestart")]
        public DateTime DateStart { get => _startDatetime; set => _startDatetime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        /// <summary>
        /// Использован ли код
        /// </summary>
        [Column("isused")]
        public bool IsUsed { get; set; } = false;
    }
}

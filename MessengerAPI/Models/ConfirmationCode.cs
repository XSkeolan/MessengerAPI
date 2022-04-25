using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("confirmationcode")]
    public class ConfirmationCode : EntityBase
    {
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
        [Column("dateend")]
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// Использован ли код
        /// </summary>
        [Column("isused")]
        public bool IsUsed { get; set; } = false;
    }
}

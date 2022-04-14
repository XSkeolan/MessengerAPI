namespace MessengerAPI.Models
{
    public class ConfirmationCode : EntityBase
    {
        /// <summary>
        /// Код для подтверждения
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Идентификатор пользователя, к которому он привязан
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Дата и время окончания действия кода
        /// </summary>
        public DateTime DateEnd { get; set; }
        /// <summary>
        /// Использован ли код
        /// </summary>
        public bool IsUsed { get; set; } = false;
    }
}

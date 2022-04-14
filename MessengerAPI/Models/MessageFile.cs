namespace MessengerAPI.Models
{
    public class MessageFile : EntityBase
    {
        /// <summary>
        /// Идентификатр сообщения к которому привязан файл
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// Идентификат файла, привязаного к сообщению
        /// </summary>
        public Guid FileId { get; set; }
    }
}

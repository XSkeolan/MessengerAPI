namespace MessengerAPI.Models
{
    public class Message : EntityBase
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Дата отправки сообщения
        /// </summary>
        public DateTime DateSend { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        public Guid From { get; set; }
        /// <summary>
        /// Получатель сообщения
        /// </summary>
        public Guid Destination { get; set; }
        /// <summary>
        /// Закреплено ли сообщение
        /// </summary>
        public bool IsPinned { get; set; } = false;
        /// <summary>
        /// Кто-то из пользователей прочитал сообщение
        /// </summary>
        public bool IsRead { get; set; } = false;
        /// <summary>
        /// Ссылка на сообщение-источник, если какое-то сообщение пересылают в другое место
        /// </summary>
        public Guid? OriginalMessageId { get; set; } = null;
        /// <summary>
        /// Ссылка на сообщение, для которого это сообщение является ответом
        /// </summary>
        public Guid? ReplyMessageId { get; set; } = null;
    }
}

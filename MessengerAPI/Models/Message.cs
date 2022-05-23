using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("messages")]
    public class Message : EntityBase
    {
        /// <summary>
        /// Текст сообщения
        /// </summary>
        [Column("text")]
        public string Text { get; set; }
        /// <summary>
        /// Дата отправки сообщения
        /// </summary>
        [Column("datesend")]
        public DateTime DateSend { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        [Column("fromwhom")]
        public Guid FromWhom { get; set; }
        /// <summary>
        /// Получатель сообщения
        /// </summary>
        [Column("destination")]
        public Guid Destination { get; set; }
        /// <summary>
        /// Закреплено ли сообщение
        /// </summary>
        [Column("ispinned")]
        public bool IsPinned { get; set; } = false;
        /// <summary>
        /// Кто-то из пользователей прочитал сообщение
        /// </summary>
        [Column("isread")]
        public bool IsRead { get; set; } = false;
        /// <summary>
        /// Ссылка на сообщение-источник, если какое-то сообщение пересылают в другое место
        /// </summary>
        [Column("originalmessageid")]
        public Guid? OriginalMessageId { get; set; } = null;
        /// <summary>
        /// Ссылка на сообщение, для которого это сообщение является ответом
        /// </summary>
        [Column("replymessageid")]
        public Guid? ReplyMessageId { get; set; } = null;
    }
}

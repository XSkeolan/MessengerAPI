using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("messagefile")]
    public class MessageFile : EntityBase
    {
        /// <summary>
        /// Идентификатр сообщения к которому привязан файл
        /// </summary>
        [Column("messageid")]
        public Guid MessageId { get; set; }
        /// <summary>
        /// Идентификат файла, привязаного к сообщению
        /// </summary>
        [Column("fileid")]
        public Guid FileId { get; set; }
    }
}

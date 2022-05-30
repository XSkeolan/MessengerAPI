using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("grouplinks")]
    public class ChatLink : EntityBase
    {
        /// <summary>
        /// Идентификатор канала на который ведет ссылка
        /// </summary>
        [Column("groupid")]
        public Guid GroupId { get; set; }
        /// <summary>
        /// Является ли ссылка одноразовой
        /// </summary>
        [Column("isonetime")]
        public bool IsOneTime { get; set; }
        /// <summary>
        /// Дата окончания действия ссылки
        /// </summary>
        [Column("dateend")]
        public DateTime DateEnd { get; set; }
    }
}

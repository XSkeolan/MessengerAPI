using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("groups")]
    public class Chat : EntityBase
    {
        /// <summary>
        /// Название чата
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
        /// <summary>
        /// Идентификатор фотографии
        /// </summary>
        [Column("photoid")]
        public Guid? PhotoId { get; set; }
        /// <summary>
        /// Дата создания чата
        /// </summary>
        [Column("datecreated")]
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Идентификатор пользователя-создателя
        /// </summary>
        [Column("creatorid")]
        public Guid CreatorId { get; set; }
        /// <summary>
        /// Роль пользователя по умолчанию
        /// </summary>
        [Column("defaulusertypeid")]
        public Guid DefaultUserTypeId { get; set; }
        public int CountUser { get;  set; }
    }
}

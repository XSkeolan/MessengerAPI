using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("channels")]
    public class Channel : EntityBase
    {
        /// <summary>
        /// Название чата
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
        /// <summary>
        /// Описание чата
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }
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
        public int CountUser { get; set; }
    }
}

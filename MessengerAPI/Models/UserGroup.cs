using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("usergroup")]
    public class UserGroup : EntityBase
    {
        /// <summary>
        /// Идентификатор пользователя 
        /// </summary>
        [Column("userid")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Идентификатор чата
        /// </summary>
        [Column("groupid")]
        public Guid ChatId { get; set; }
        /// <summary>
        /// Идентификатор типа пользователя в чате
        /// </summary>
        [Column("usertypeid")]
        public Guid UserTypeId  { get; set; }
    }
}

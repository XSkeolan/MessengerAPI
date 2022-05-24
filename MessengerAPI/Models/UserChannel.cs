using System.ComponentModel.DataAnnotations.Schema;

namespace MessengerAPI.Models
{
    [Table("userchannel")]
    public class UserChannel : EntityBase
    {
        /// <summary>
        /// Идентификатор канала
        /// </summary>
        [Column("channelid")]
        public Guid ChannelId { get; set; }
        /// <summary>
        /// Идентификатор пользователя, вошедшего в канал
        /// </summary>
        [Column("userid")]
        public Guid UserId { get; set; }
    }
}
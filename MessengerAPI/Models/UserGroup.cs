namespace MessengerAPI.Models
{
    public class UserGroup : EntityBase
    {
        /// <summary>
        /// Идентификатор пользователя 
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public Guid ChatId { get; set; }
        /// <summary>
        /// Идентификатор типа пользователя в чате
        /// </summary>
        public Guid UserTypeId  { get; set; }
    }
}

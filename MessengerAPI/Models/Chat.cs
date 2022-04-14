namespace MessengerAPI.Models
{
    public class Chat : EntityBase
    {
        /// <summary>
        /// Название чата
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание чата
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Идентификатор фотографии
        /// </summary>
        public Guid? PhotoId { get; set; }
        /// <summary>
        /// Дата создания чата
        /// </summary>
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Идентификатор пользователя-создателя
        /// </summary>
        public Guid CreatorId { get; set; }
    }
}

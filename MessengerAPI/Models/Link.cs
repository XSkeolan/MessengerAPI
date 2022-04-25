namespace MessengerAPI.Models
{
    public class Link : EntityBase
    {
        /// <summary>
        /// Идентификатор канала, на который ведет ссылка
        /// </summary>
        public Guid GroupId { get; set; }
        /// <summary>
        /// Дата создания ссылки
        /// </summary>
        public DateTime DateCreate { get; set; }
        /// <summary>
        /// Одноразовая ссылка
        /// </summary>
        public bool OneTime { get; set; }
        /// <summary>
        /// Активная ли ссылка
        /// </summary>
        public bool IsActive { get; set; }
    }
}

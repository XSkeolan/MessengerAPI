namespace MessengerAPI.DTOs
{
    public class ChatRequest
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Идентификатор фотографии
        /// </summary>
        public Guid? PhotoId { get; set; }
    }
}
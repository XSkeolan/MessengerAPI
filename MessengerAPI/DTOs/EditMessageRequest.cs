namespace MessengerAPI.DTOs
{
    public class EditMessageRequest
    {
        /// <summary>
        /// Идетификатор сообщения для изменения
        /// </summary>
        public Guid EditableMessageId { get; set; }
        /// <summary>
        /// Измененный текст
        /// </summary>
        public string ModifiedText { get; set; }
        /// <summary>
        /// Измененные вложения
        /// </summary>
        public IEnumerable<byte[]> ModifiedAttachment { get; set; }
    }
}

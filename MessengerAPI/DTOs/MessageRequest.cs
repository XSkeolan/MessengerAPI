namespace MessengerAPI.DTOs
{
    public class MessageRequest
    {
        /// <summary>
        /// Текст отправляемого сообщения
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Идентификатор пункта назначения сообщения
        /// </summary>
        public Guid Destination { get; set; }
        /// <summary>
        /// Файлы приложенные к сообщению
        /// </summary>
        public IEnumerable<byte[]>? Attachment { get; set; }
    }
}

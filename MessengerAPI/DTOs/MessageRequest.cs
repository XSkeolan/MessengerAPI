using MessengerAPI.Models;
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
        /// Тип пункта назначения сообщения
        /// Возможные значение:
        /// 0 - чат;
        /// 1 - пользователь;
        /// </summary>
        public DestinationType DestinationType { get; set; }
        /// <summary>
        /// Идентификатор сообщения, на которое этим сообщением хотят ответить
        /// </summary>
        public Guid? ReplyMessageId { get; set; }
    }
}

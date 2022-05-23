﻿namespace MessengerAPI.DTOs
{
    public class DialogInfoResponse
    {
        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Название чата
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фотография чта
        /// </summary>
        public Guid? Photo { get; set; }
        /// <summary>
        /// Последнее сообщение в чате
        /// </summary>
        public string LastMessageText { get; set; }
        /// <summary>
        /// Сколько времени прошло с даты отправления последнего сообщения
        /// </summary>
        public DateTime? LastMessageDateSend { get; set; }
    }
}

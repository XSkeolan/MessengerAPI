﻿namespace MessengerAPI.DTOs
{
    public class ChatResponse
    {
        /// <summary>
        /// Идентификатор нового чата
        /// </summary>
        public Guid ChatId { get; set; }
        /// <summary>
        /// Название чата
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание чата
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Фотография чата
        /// </summary>
        public Guid? Photo { get; set; }
        /// <summary>
        /// Количество пользователей в канале
        /// </summary>
        public int CountUsers { get; set; }
    }
}

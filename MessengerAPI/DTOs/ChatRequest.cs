using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.DTOs
{
    public class ChatRequest
    {
        /// <summary>
        /// Название чата
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание чата
        /// </summary>
        public string? Description { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.DTOs
{
    public class BaseSearchRequest
    {
        /// <summary>
        /// Строка, которую должен содержать никнейм пользователя
        /// </summary>
        [FromQuery]
        public string SubString { get; set; }
        /// <summary>
        /// Максимальное количество пользователей для результирующего набора
        /// </summary>
        [FromQuery]
        public int LimitResult { get; set; }
    }
}

namespace MessengerAPI.DTOs
{
    public class BaseSearchRequest
    {
        /// <summary>
        /// Строка, которую должен содержать никнейм пользователя
        /// </summary>
        public string SubString { get; set; }
        /// <summary>
        /// Максимальное количество пользователей для результирующего набора
        /// </summary>
        public int LimitResult { get; set; }
    }
}

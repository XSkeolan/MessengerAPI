namespace MessengerAPI.DTOs
{
    public class SendCodeResponse
    {
        /// <summary>
        /// Хэш кода, который был отправлен на email
        /// </summary>
        public string EmailCodeHash { get; set; }
        /// <summary>
        /// Время действия кода
        /// </summary>
        public int Timeout { get; set; }
    }
}

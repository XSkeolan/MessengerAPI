namespace MessengerAPI.DTOs
{
    public class SignInResponse
    {
        /// <summary>
        /// Токен авторизации
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Время жизни токена в секундах
        /// </summary>
        public int Expiries { get; set; }
    }
}

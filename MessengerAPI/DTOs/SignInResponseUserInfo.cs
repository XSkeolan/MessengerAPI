namespace MessengerAPI.DTOs
{
    public class SignInResponseUserInfo
    {
        /// <summary>
        /// Токен авторизации
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Время жизни токена в секундах
        /// </summary>
        public int Expiries { get; set; }
        /// <summary>
        /// Пользователь вошедший в учетную запись
        /// </summary>
        public UserResponse User { get; set; }
    }
}

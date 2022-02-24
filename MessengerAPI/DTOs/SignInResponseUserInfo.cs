namespace MessengerAPI.DTOs
{
    public class SignInResponseUserInfo
    {
        /// <summary>
        /// Идентификатор сессии в которую вошел пользователь
        /// </summary>
        public Guid SessionId { get; set; }
        /// <summary>
        /// Пользователь вошедший в учетную запись
        /// </summary>
        public UserResponse User { get; set; }
    }
}

namespace MessengerAPI.DTOs
{
    public class UserCreateResponse : BaseUserResponse
    {
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        public string Phonenumber { get; set; }
        /// <summary>
        /// Подтвержден ли email
        /// </summary>
        public bool IsConfirmed { get; set; }
    }
}

namespace MessengerAPI.DTOs
{
    public class ShortUserResponse : BaseUserResponse
    {
        /// <summary>
        /// Тип пользователя в чате
        /// </summary>
        public Guid UserTypeId { get; set; }
    }
}
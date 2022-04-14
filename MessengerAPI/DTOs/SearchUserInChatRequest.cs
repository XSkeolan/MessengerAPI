namespace MessengerAPI.DTOs
{
    public class SearchUserInChatRequest : BaseSearchRequest
    {
        /// <summary>
        /// Идентификатор чата в котором нужно искать пользоватедя
        /// </summary>
        public Guid ChatId { get; set; }
    }
}

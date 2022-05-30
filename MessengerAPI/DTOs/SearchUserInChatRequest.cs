using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.DTOs
{
    public class SearchUserInChatRequest : BaseSearchRequest
    {
        /// <summary>
        /// Идентификатор чата в котором нужно искать пользоватедя
        /// </summary>
        [FromQuery]
        public Guid ChatId { get; set; }
    }
}

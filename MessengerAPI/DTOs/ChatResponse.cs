namespace MessengerAPI.DTOs
{
    public class ChatResponse
    {
        /// <summary>
        /// Идентификатор нового чата
        /// </summary>
        public Guid ChatId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid ChatType { get; set; }
        public IEnumerable<UserResponse> InviteUsers { get;set; }
    }
}

using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserChatRepository :IRepository<UserGroup>
    {
        public Task<IEnumerable<Guid>> GetUserChatsAsync(Guid userId);
        public Task<IEnumerable<Guid>> GetChatUsers(Guid chatId);
        public Task<UserGroup?> GetByChatAndUserAsync(Guid chatId, Guid userId);
    }
}

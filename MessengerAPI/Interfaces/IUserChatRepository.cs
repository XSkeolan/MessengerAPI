using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserChatRepository :IRepository<UserGroup>
    {
        public Task<IEnumerable<Guid>> GetUserChats(Guid userId);
        public Task<IEnumerable<Guid>> GetChatAdmins(Guid chatId);
        public Task<IEnumerable<Guid>> GetChatUsers(Guid chatId);
        public Task<UserGroup?> GetByChatAndUserAsync(Guid chatId, Guid userId);
        public Task UpdateAsync(Guid id, Guid userTypeId);
    }
}

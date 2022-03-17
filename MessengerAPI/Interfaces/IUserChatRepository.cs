using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserChatRepository :IRepository<UserGroup>
    {
        public Task<IEnumerable<Guid>> GetUserChats(Guid userId);
        public Task<Guid> GetChatAdmin(Guid chatId);
        public Task<IEnumerable<Guid>> GetChatUsers(Guid chatId);
    }
}

using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserChatRepository : BaseRepository<UserGroup>, IUserChatRepository
    {
        public UserChatRepository(IOptions<Connections> options) : base(options) { }

        public async Task<UserGroup?> GetByChatAndUserAsync(Guid chatId, Guid userId)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(
                cond.AndOperation(
                    Builder.Condition.EqualOperation("userid", userId, EqualOperations.Equal),
                    Builder.Condition.EqualOperation("groupid", chatId, EqualOperations.Equal)),
                cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond)).FirstOrDefault();
        }

        public async Task<IEnumerable<Guid>> GetUserChatsAsync(Guid userId)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.EqualOperation("userid", userId, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond)).Select(x => x.GroupId);
        }

        public async Task<IEnumerable<Guid>> GetChatUsersAsync(Guid chatId)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.EqualOperation("groupid", chatId, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond)).Select(x => x.UserId);
        }
    }
}

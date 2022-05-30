using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(IOptions<Connections> options, IServiceContext serviceContext) : base(options, serviceContext) { }

        public async Task<IEnumerable<Chat>> GetChatsByNameAsync(string name)
        {
            ConditionBuilder cond = Builder.Condition;
            cond.AndOperation(cond.LikeOperation("name", $"%{name}%"), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return await GetByConditions(cond);
        }
    }
}

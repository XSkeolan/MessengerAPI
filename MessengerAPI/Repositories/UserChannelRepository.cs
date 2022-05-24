using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserChannelRepository : BaseRepository<UserChannel>, IUserChannelRepository
    {
        public UserChannelRepository(IOptions<Connections> options, IServiceContext serviceContext) : base(options, serviceContext)
        {
        }

        public async Task<IEnumerable<UserChannel>> GetChannelUsersAsync(Guid channelId)
        {
            ConditionBuilder cond = Builder.Condition;

            cond = cond.AndOperation(cond.EqualOperation("channelid", channelId, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond));
        }
    }
}

using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ChannelRepository : BaseRepository<Channel>, IChannelRepository
    {
        public ChannelRepository(IOptions<Connections> options) : base(options)
        {
            
        }

        public async Task<IEnumerable<Channel>> GetChannelByNameAsync(string name)
        {
            ConditionBuilder cond = Builder.Condition;
            cond.AndOperation(cond.LikeOperation("name", "%name%"), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return await GetByConditions(cond);
        }
    }
}

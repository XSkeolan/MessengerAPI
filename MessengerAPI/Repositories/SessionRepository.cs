using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using Microsoft.Extensions.Options;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(IOptions<Connections> options) : base(options) { }

        public async Task<Session?> GetUnfinishedOnDeviceAsync(string device, DateTime endTimeSession)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.EqualOperation("devicename", device, EqualOperations.Equal), cond.EqualOperation("dateend", endTimeSession, EqualOperations.MoreEqual));

            return (await GetByConditions(cond)).FirstOrDefault();
        }
    }
}

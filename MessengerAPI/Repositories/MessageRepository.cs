using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(IOptions<Connections> options, IServiceContext serviceContext) : base(options, serviceContext) { }

        public async Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId)
        {
            ConditionBuilder condition = Builder.Condition.AndOperation(
                Builder.Condition.EqualOperation("Destination", destinationId, EqualOperations.Equal), 
                Builder.Condition.EqualOperation("IsDeleted", false, EqualOperations.Equal));

            return await GetByConditions(condition);
        }

        public async Task<Message?> GetLastMessage(Guid destinationId)
        {
            IEnumerable<Message> destinationMessages = await GetMessagesByDestination(destinationId);
            return destinationMessages.OrderBy(msg => msg.DateSend).FirstOrDefault();
        }
    }
}

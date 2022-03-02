using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(Message message)
        {
            message.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO messages (text, datasend, \"from\", destination, destinationtype, originalmessageid) " +
                    "VALUES(@Text, @DateSend, @From, @Destination, @DestinationType, @OriginalMessageId) RETURNING id", message);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE messages SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<Message?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<Message> groups = await conn.QueryAsync<Message>("SELECT * FROM messages WHERE id=@Id AND isdeleted=false", new { id });
                return groups.FirstOrDefault();
            });
        }

        public async Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Message>("SELECT * FROM messages WHERE destination=@Destination AND isdeleted=false", new { Destination = destinationId });
            });
        }
    }
}

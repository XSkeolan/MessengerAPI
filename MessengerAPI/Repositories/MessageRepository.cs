using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(IOptions<Connections> options, IServiceContext serviceContext) : base(options, serviceContext) { }

        public override async Task CreateAsync(Message message)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO messages (id, text, datesend, \"from\", destination, ispinned, isread, originalmessageid, replymessageid, isdeleted) " +
                    "VALUES(@Id, @Text, @DateSend, @From, @Destination, @IsPinned, @IsRead, @OriginalMessageId, @ReplyMessageId, @IsDeleted)", message);
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

        public async Task UpdateAsync(Guid id, bool isPinned)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE messages SET ispinned=@IsPinned WHERE id=@Id AND isdeleted=false", new { Id = id, IsPinned = isPinned });
            });
        }

        public async Task UpdateAsync(Guid id, string text)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE messages SET text=@Text WHERE id=@Id AND isdeleted=false", new { Id = id, Text = text });
            });
        }

        public async Task UpdateAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE messages SET isread=true WHERE id=@Id AND isdeleted=false", id);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE messages SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public async Task<IEnumerable<Message>> GetMessagesByDestination(Guid destinationId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Message>("SELECT * FROM messages WHERE destination=@Destination AND isdeleted=false", new { Destination = destinationId });
            });
        }

        public Task<Message> GetLastInChatAsync(Guid chatId)
        {
            throw new NotImplementedException();
        }

        //public Task<Message> GetLastInChatAsync(Guid chatId)
        //{

        //}
    }
}

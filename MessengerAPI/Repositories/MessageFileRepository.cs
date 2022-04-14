using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class MessageFileRepository : BaseRepository<MessageFile>, IMessageFileRepository
    {
        public MessageFileRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(MessageFile entity)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO messagefile (id, messageid, fileid, isdeleted) " +
                    "VALUES(@Id, @MessageId, @FileId, @IsDeleted)", entity);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE messagefile SET isdeleted=@IsDeleted WHERE id=@Id", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<MessageFile?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<MessageFile>("SELECT * FROM messagefile WHERE id=@Id", new { Id = id });
            });
        }
    }
}

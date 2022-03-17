using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ChatTypeRepository : BaseRepository<ChatType>, IChatTypeRepository
    {
        public ChatTypeRepository(IOptions<Connections> options) : base(options) { }

        public async override Task CreateAsync(ChatType chatType)
        {
            chatType.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO chattypes (type)" +
                    "VALUES(@Type) RETURNING id", new { Type = chatType.Type });
            });
        }

        public async override Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE chattypes SET isdeleted=true WHERE id=@Id AND isdeleted=false", new { Id = id });
            });
        }

        public async override Task<ChatType> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ChatType>("SELECT * FROM chattypes WHERE id=@Id and isdeleted=false", new { Id = id });
            });
        }

        public async Task<ChatType> GetByTypeName(string type)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ChatType>("SELECT * FROM chattypes WHERE type=@Type and isdeleted=false", new { Type = type });
            });
        }
    }
}

using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using Dapper;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(Chat chat)
        {
            chat.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO groups (name, description, administrator, datecreate, ischannel) " +
                    "VALUES(@Name, @Description, @Administrator, @DateCreate, @IsChannel) RETURNING id",
                    new { Name = chat.Name, Description = chat.Description, /*Photo = chat.Photo,*/ Administrator=chat.Administrator, DateCreate = chat.Created, IsChannel = false });
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE groups SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<Chat?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Chat>("SELECT * FROM groups WHERE id=@Id AND isdeleted=false", new { id });
            });
        }
    }
}

using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserChatRepository : BaseRepository<UserChat>, IUserChatRepository
    {
        public UserChatRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(UserChat userChat)
        {
            userChat.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO usergroup (userId, groupId, usertype) " +
                "VALUES(@UserId, @ChatId, @UserTypeId) RETURNING id", userChat);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE usergroup SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<UserChat> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserChat>("SELECT * FROM usergroup WHERE id=@Id AND isdeleted=false", new { id });
            });
        }
    }
}

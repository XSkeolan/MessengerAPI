using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserChatRepository : BaseRepository<UserGroup>, IUserChatRepository
    {
        public UserChatRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(UserGroup userChat)
        {
            userChat.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO usergroup (userid, groupid, usertypeid) " +
                "VALUES(@UserId, @GroupId, @UserTypeId) RETURNING id", userChat);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE usergroup SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<UserGroup> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserGroup>("SELECT * FROM usergroup WHERE id=@Id AND isdeleted=false", new { id });
            });
        }

        public async Task<IEnumerable<Guid>> GetUserChats(Guid userId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Guid>("SELECT groupid FROM usergroup WHERE userid=@Id AND isdeleted=false", new { Id=userId });
            });
        }
    }
}

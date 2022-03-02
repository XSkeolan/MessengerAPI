using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserGroupRepository : BaseRepository<UserGroup>, IUserChatRepository
    {
        public UserGroupRepository(IOptions<Connections> options) : base(options) { }

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
    }
}

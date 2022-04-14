using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserChatRepository : BaseRepository<UserGroup>, IUserChatRepository
    {
        public UserChatRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(UserGroup userChat)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO usergroup (id, userid, groupid, usertypeid, isdeleted) " +
                "VALUES(@Id, @UserId, @ChatId, @UserTypeId, @IsDeleted)", userChat);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE usergroup SET isdeleted=true WHERE id=@Id AND isdeleted=false", new { Id = id });
            });
        }

        public override async Task<UserGroup?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserGroup>("SELECT * FROM usergroup WHERE id=@Id AND isdeleted=false", new { id });
            });
        }

        public async Task<UserGroup?> GetByChatAndUserAsync(Guid chatId, Guid userId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserGroup>("SELECT * FROM usergroup WHERE userid=@UserId AND groupid=@GroupId AND isdeleted=false", new { UserId=userId, GroupId=chatId });
            });
        }

        public async Task<IEnumerable<Guid>> GetUserChatsAsync(Guid userId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Guid>("SELECT groupid FROM usergroup WHERE userid=@Id AND isdeleted=false", new { Id=userId });
            });
        }

        public async Task<IEnumerable<Guid>> GetChatAdmins(Guid chatId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Guid>("SELECT userid FROM usergroup JOIN usertypes ON usertypeid=usertypes.id WHERE groupId=@ChatId AND type=@Type", new { ChatId = chatId, Type="admin" });
            });
        }

        public async Task<IEnumerable<Guid>> GetChatUsers(Guid chatId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Guid>("SELECT userid FROM usergroup WHERE groupid=@Id AND isdeleted=false", new { Id = chatId });
            });
        }

        public async Task UpdateAsync(Guid id, Guid userTypeId)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE usergroup SET usertypeid=@UserTypeId WHERE id=@Id AND isdeleted=false", new { Id = id, UserTypeId = userTypeId });
            });
        }
    }
}

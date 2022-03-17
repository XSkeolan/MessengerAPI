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
            userChat.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO usergroup (userid, groupid, usertypeid) " +
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

        public async Task<Guid> GetChatAdmin(Guid chatId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("SELECT userid FROM usergroup JOIN usertypes ON usertypeid=usertypes.id WHERE groupId=@ChatId AND type=@Type", new { ChatId = chatId, Type="admin" });
            });
        }

        public async Task<IEnumerable<Guid>> GetChatUsers(Guid chatId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<Guid>("SELECT userid FROM usergroup WHERE chatid=@Id AND isdeleted=false", new { Id = chatId });
            });
        }
    }
}

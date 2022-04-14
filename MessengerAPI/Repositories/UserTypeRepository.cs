using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using Dapper;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class UserTypeRepository : BaseRepository<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(UserType userType)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO usertypes (id, typename, permissions, prioritylevel, isdefault, isdeleted) " +
                    "VALUES(@Id, @TypeName, @Permissions, @PriorityLevel, @IsDefault, @IsDeleted)", userType);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE usertypes SET isdeleted=true" +
                    "FROM groups WHERE id=@Id AND isdeleted=false", 
                    new { Id = id });
            });
        }

        public override async Task<UserType?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserType>("SELECT * FROM usertypes " +
                    "WHERE id=@Id AND isdeleted=false", 
                    new { id });
            });
        }

        public async Task<UserType?> GetByTypeNameAsync(string typeName)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserType>("SELECT * FROM usertypes " +
                    "WHERE typename=@Type AND isdeleted=false", 
                    new { Type = typeName });
            });
        }


        public async Task<UserType> GetUserTypeInChatAsync(Guid userId, Guid chatId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserType>("SELECT usertypes.id, typename, usertypes.isdeleted " +
                    "FROM usertypes JOIN usergroup ON usertypes.id=usergroup.usertypeid " +
                    "WHERE userid=@UserId AND groupid=@GroupId", 
                    new { UserId = userId, GroupId = chatId });
            });
        }

        public async Task<UserType> GetDefaultType()
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserType>("SELECT * FROM usertypes WHERE isdefault=true AND isdeleted=false");
            });
        }

        public async Task<IEnumerable<UserType>> GetAll()
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<UserType>("SELECT * FROM usertypes WHERE isdeleted=false");
            });
        }
    }
}

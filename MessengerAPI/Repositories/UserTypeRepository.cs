using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using Dapper;

namespace MessengerAPI.Repositories
{
    public class UserTypeRepository : BaseRepository<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(UserType userType)
        {
            userType.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO usertypes (type) VALUES(@Type) RETURNING id", 
                    new { Type = userType.Type });
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE usertypes SET isdeleted=@IsDeleted FROM groups WHERE id=@Id AND isdeleted=false", 
                    new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<UserType> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<UserType>("SELECT * FROM usertypes WHERE id=@Id AND isdeleted=false", new { id });
            });
        }

        public async Task<Guid> GetIdByTypeName(string typeName)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<Guid> ids = await conn.QueryAsync<Guid>("SELECT id FROM usertypes WHERE type=@Type AND isdeleted=false", new { Type=typeName });
                if (ids.Count() > 1)
                    throw new InvalidOperationException();
                return ids.FirstOrDefault();
            });
        }
    }
}

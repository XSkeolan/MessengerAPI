using Dapper;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ConfirmationCodeRepository : BaseRepository<ConfirmationCode>, IConfirmationCodeRepository
    {
        public ConfirmationCodeRepository(IOptions<Connections> options) : base(options) { }

        public async override Task CreateAsync(ConfirmationCode confirmationCode)
        {
            confirmationCode.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO confirmationcode (code, userid, dateend) " +
                "VALUES(@Code, @UserId, @DateEnd) RETURNING id", confirmationCode);
            });
        }

        public async override Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE confirmationcode SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public async override Task<ConfirmationCode?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ConfirmationCode>("SELECT * FROM confirmationcode WHERE id=@Id AND isdeleted=false", new { id });
            });
        }

        public async Task<bool> UnUsedCodeExists(string code)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ConfirmationCode>("SELECT * FROM confirmationcode WHERE code=@Code AND isused=false AND isdeleted=false", new { Code = code }) != null;
            });
        }

        public async Task<bool> UserHasUnUsedCode(Guid userId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ConfirmationCode>("SELECT * FROM confirmationcode WHERE userid=@UserId AND isdeleted=false AND isused=false", new { UserId = userId }) != null;
            });
        }
    }
}

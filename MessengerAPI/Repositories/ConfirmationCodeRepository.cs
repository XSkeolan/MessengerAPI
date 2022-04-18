﻿using Dapper;
using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ConfirmationCodeRepository : BaseRepository<ConfirmationCode>, IConfirmationCodeRepository
    {
        public ConfirmationCodeRepository(IOptions<Connections> options) : base(options) { }

        public async override Task CreateAsync(ConfirmationCode confirmationCode)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO confirmationcode (id, code, userid, isused, dateend, isdeleted) " +
                "VALUES(@Code, @UserId, @DateEnd)", confirmationCode);
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

        public async Task<bool> UnUsedCodeExists(string codeHash)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ConfirmationCode>("SELECT * FROM confirmationcode WHERE code=@Code AND isused=false AND isdeleted=false", new { Code = codeHash }) != null;
            });
        }

        public async Task<ConfirmationCode> GetUnsedCodeByUser(Guid userId)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<ConfirmationCode>("SELECT * FROM confirmationcode WHERE userid=@UserId AND isdeleted=false AND isused=false", new { UserId = userId });
            });
        }

        public async Task UpdateAsync(Guid id, string codeHash)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE confirmationcode SET code=@Code WHERE id=@Id AND isdeleted=false", new { Code = codeHash, Id = id });
            });
        }

        public async Task UpdateAsync(Guid id, bool isused)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE confirmationcode SET isused=@IsUsed WHERE id=@Id AND isdeleted=false", new { IsUsed = isused, Id = id });
            });
        }
    }
}

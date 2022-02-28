﻿using MessengerAPI.Models;
using Dapper;
using MessengerAPI.Interfaces;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(Session session)
        {
            session.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO Sessions (datestart, userid, devicename) " +
                    "VALUES(@DateStart, @UserId, @DeviceName) RETURNING id", session);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Sessions SET dateend=@DateEnd WHERE id=@Id AND dateend is null", new { DateEnd = DateTime.Now, Id = id });
            });
        }

        public override async Task<Session?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<Session> sessions = await conn.QueryAsync<Session>("SELECT * FROM Sessions WHERE id=@Id", new { id });
                return sessions.FirstOrDefault();
            });
        }
    }
}

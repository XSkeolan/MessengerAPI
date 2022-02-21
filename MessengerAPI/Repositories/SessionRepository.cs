using MessengerAPI.Models;
using System.Data;
using Npgsql;
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
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                IEnumerable<Guid> id = await connection.QueryAsync<Guid>("INSERT INTO Sessions (datestart, userid, devicename) " +
                    "VALUES(@DateStart, @UserId, @DeviceName) RETURNING id", session);
                session.Id = id.FirstOrDefault();
            }
        }

        public override Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task FinishSessionAsync(Guid id)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                await connection.ExecuteAsync("UPDATE Sessions SET dateend=@dateEnd WHERE id=@Id", new { DateEnd = DateTime.Now, Id = id });
            }
        }

        public override Task<IEnumerable<Session>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<Session?> GetAsync(Guid id)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                IEnumerable<Session> sessions = await connection.QueryAsync<Session>("SELECT * FROM Sessions WHERE id=@Id", new { id });
                return sessions.FirstOrDefault();
            }
        }

        public override Task UpdateAsync(Session entity)
        {
            throw new NotImplementedException();
        }
    }
}

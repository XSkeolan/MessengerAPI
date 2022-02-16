using MessengerAPI.Models;
using System.Data;
using Npgsql;
using Dapper;
using MessengerAPI.Interfaces;

namespace MessengerAPI.Repositories
{
    public class SessionRepository : BaseRepository, ISessionRepository
    {
        public SessionRepository(string connectionString) : base(connectionString) { }

        public async Task CreateAsync(Session session)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                IEnumerable<Guid> id = await connection.QueryAsync<Guid>("INSERT INTO Sessions (datestart, userid, devicename) " +
                    "VALUES(@DateStart, @UserId, @DeviceName) RETURNING id", session);
                session.Id = id.FirstOrDefault();
            }
        }

        public async Task FinishSessionAsync(Guid id)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                await connection.ExecuteAsync("UPDATE Sessions SET dateend=@dateEnd WHERE id=@Id", new { DateEnd = DateTime.Now, Id = id });
            }
        }

        public async Task<Session?> GetAsync(Guid id)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                IEnumerable<Session> sessions = await connection.QueryAsync<Session>("SELECT * FROM Sessions WHERE id=@Id", new { id });
                return sessions.FirstOrDefault();
            }
        }
    }
}

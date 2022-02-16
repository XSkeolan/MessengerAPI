using MessengerAPI.Models;
using System.Data;
using Npgsql;
using Dapper;

namespace MessengerAPI.Repos
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _connectionString;
        public SessionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Session session)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                Guid? id = connection.Query<Guid>("INSERT INTO Sessions (datestart, userid, devicename) " +
                    "VALUES(@DateStart, @UserId, @DeviceName) RETURNING id", session).FirstOrDefault();
                session.Id = id.Value;
            }
        }

        public void End(Guid id)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute("UPDATE Sessions SET dateend=@dateEnd WHERE id=@Id", new { DateEnd = DateTime.Now, Id = id });
            }
        }

        public Session? Get(Guid id)
        {
            using(IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Session>("SELECT * FROM Sessions WHERE id=@Id", new { id }).FirstOrDefault();
            }
        }
    }
}

using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly string _connectionString;
        public BaseRepository(IOptions<Connections> options)
        {
            _connectionString = options.Value.MessengerAPI;
        }

        protected async Task<TResult> Execute<TResult>(Func<IDbConnection, Task<TResult>> func)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return await func(conn);
        }

        public abstract Task CreateAsync(TEntity entity);
        public abstract Task DeleteAsync(Guid id);
        public abstract Task<TEntity> GetAsync(Guid id);
    }
}

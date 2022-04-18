using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly string _connectionString;
        protected readonly IServiceContext _serviceContext;

        public BaseRepository(IOptions<Connections> options, IServiceContext serviceContext)
        {
            _connectionString = options.Value.MessengerAPI;
            _serviceContext = serviceContext;
        }

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

        protected async Task<TResult> ExecuteByContext<TResult>(Func<IDbConnection, string, Task<TResult>> func)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string condition = $"WHERE userid={_serviceContext.UserId}";
            return await func(conn, condition);
        }

        protected async Task<TResult> ExecuteWithUserGroup<TResult>(Func<IDbConnection, string, Task<TResult>> func, string fieldName) //можно и enum
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string from = $"AS tempTable JOIN usergroup ON tempTable.id={fieldName} AND userid='{_serviceContext.UserId}' AND usergroup.isdeleted=false";
            return await func(conn, from);
        }

        protected async Task<TResult> ExecuteWithConnectedTable<TResult, TTable>(Func<IDbConnection, string, Task<TResult>> func, string fieldName) //можно и enum
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string from = $"AS tempTable JOIN {typeof(TTable).Name.ToLower()} ON tempTable.id={fieldName} AND userid='{_serviceContext.UserId}' AND {typeof(TTable).Name.ToLower()}.isdeleted=false";
            return await func(conn, from);
        }

        protected string QueryUpdate(string tabelField, string value, Guid id) //
        {
            return $"UPDATE table SET {tabelField}='{value}' WHERE Id='{id}'";
        }


        public abstract Task CreateAsync(TEntity entity);
        public abstract Task DeleteAsync(Guid id);
        public abstract Task<TEntity?> GetAsync(Guid id);
    }
}

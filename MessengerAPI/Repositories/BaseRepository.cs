using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Options;
using MessengerAPI.Options;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Dapper;

namespace MessengerAPI.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly string _connectionString;
        protected readonly IServiceContext _serviceContext;
        private string TableName => typeof(TEntity).GetCustomAttribute<TableAttribute>().Name;

        public BaseRepository(IOptions<Connections> options, IServiceContext serviceContext)
        {
            _connectionString = options.Value.MessengerAPI;
            _serviceContext = serviceContext;
        }

        public BaseRepository(IOptions<Connections> options)
        {
            _connectionString = options.Value.MessengerAPI;
        }

        public async Task CreateAsync(TEntity entity)
        {
            await Execute(async (conn) =>
            {
                string fieldToInsert = "";
                string valuesToInsert = "";
                IEnumerable<PropertyInfo> propertyInfos = typeof(TEntity).GetProperties().Where(x => x.GetCustomAttribute<ColumnAttribute>() != null);

                foreach (var property in propertyInfos)
                {
                    fieldToInsert += property.Name.ToLower() + ' ';
                    valuesToInsert += '@' + property.Name + ' ';
                }

                return await conn.ExecuteAsync($"INSERT INTO {TableName} ({fieldToInsert}) VALUES({valuesToInsert})", entity);
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync($"UPDATE {TableName} SET isdeleted=true WHERE id=@Id AND isdeleted=false", new { Id = id });
            });
        }

        private async Task<TResult> Execute<TResult>(Func<IDbConnection, Task<TResult>> func)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return await func(conn);
        }

        public async Task<TEntity> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<TEntity>($"SELECT * FROM {TableName} WHERE id=@Id", new { Id = id });
            });
        }

        public async Task<IEnumerable<TEntity>> GetByConditions(IDictionary<string, string> conditions)
        {
            return await Execute(async (conn) =>
            {
                string queryCond = "";
                foreach (var cond in conditions)
                {
                    queryCond += cond.Key + "=" + cond.Value + ' ';

                    if (conditions.Keys.Last() != cond.Key)
                    {
                        queryCond += "AND ";
                    }
                }
                Console.WriteLine(queryCond);
                return await conn.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE {queryCond}");
            });
        }

        public async Task<IEnumerable<TEntity>> GetByServiceContextAsync(string fieldName)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE {fieldName}=@Context", new { Context = _serviceContext.UserId });
            });
        }

        public async Task<IEnumerable<TEntity>> GetWithConnectedTableAsync(IEnumerable<string> selectedFields, Guid id, string connectedTableName, IDictionary<string,string> connectedFields)
        {
            string select = "SELECT ";
            string join = $"JOIN {connectedTableName} ON ";

            foreach (string field in selectedFields)
            {
                select += field + ' ';
            }
            foreach (var field in connectedFields)
            {
                join += field.Key + "=" + field.Value + " AND ";
            }
            Console.WriteLine(join);

            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<TEntity>($"{select} FROM (SELECT * FROM {TableName} WHERE id=@Id AND isdeleted=false) AS tempTable {join} {connectedTableName}.isdeleted=false");
            });
        }

        public async Task UpdateAsync(Guid id, string field, string value)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync($"UPDATE {TableName} SET {field}=@Field WHERE id=@Id AND isdeleted=false", new { Field = value, Id = id });
            });
        }
    }
}

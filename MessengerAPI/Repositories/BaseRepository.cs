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

        public virtual IDictionary<string, object> EntityToDictionary(TEntity entity)
        {
            var result = new Dictionary<string, object>();
            var properties = typeof(TEntity).GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetCustomAttribute<NotMappedAttribute>() == null &&
                    properties[i].GetCustomAttribute<ColumnAttribute>() != null)
                {
                    result.Add(properties[i].GetCustomAttribute<ColumnAttribute>().Name, properties[i].GetValue(entity));
                }
            }

            return result;
        }

        public async Task CreateAsync(IDictionary<string, object> entity)
        {
            await Execute(async (conn) =>
            {
                string fieldToInsert = string.Join(", ", entity.Keys).ToLower();
                string valuesToInsert = '@' + string.Join(", @", entity.Keys);

                return await conn.ExecuteAsync($"INSERT INTO {TableName} ({fieldToInsert}) VALUES({valuesToInsert})", entity);
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                ConditionBuilder conditionBuilder = Builder.Condition;
                var query = conditionBuilder.AndOperation(
                    conditionBuilder.EqualOperation("id", id, EqualOperations.Equal),
                    conditionBuilder.EqualOperation("isdeleted", false, EqualOperations.Equal)).Build();
                return await conn.ExecuteAsync($"UPDATE {TableName} SET isdeleted=true WHERE {query.Query}", query.Args);
            });
        }

        private async Task<TResult> Execute<TResult>(Func<IDbConnection, Task<TResult>> func)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return await func(conn);
        }

        public async Task<TEntity?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                ConditionBuilder conditionBuilder = Builder.Condition;
                var query = conditionBuilder.AndOperation(
                    conditionBuilder.EqualOperation("id", id, EqualOperations.Equal),
                    conditionBuilder.EqualOperation("isdeleted", false, EqualOperations.Equal)).Build();
                return await conn.QuerySingleOrDefaultAsync<TEntity>($"SELECT * FROM {TableName} WHERE {query.Query}", query.Args);
            });
        }

        public async Task<IEnumerable<TEntity>> GetByConditions(ConditionBuilder conditions)
        {
            return await Execute(async (conn) =>
            {
                var result = conditions.Build();
                foreach (var item in result.Args.Values)
                {
                    Console.WriteLine(item);
                }

                IEnumerable<TEntity> f = await conn.QueryAsync<TEntity>($"SELECT * FROM {TableName} WHERE {result.Query}", result.Args);
                return f;
            });
        }

        public async Task UpdateAsync(Guid id, string field, object value)
        {
            ConditionBuilder builder = Builder.Condition.AndOperation(
                Builder.Condition.EqualOperation("id", id, EqualOperations.Equal),
                Builder.Condition.EqualOperation("isdeleted", false, EqualOperations.Equal));
            var result = builder.Build();

            await Execute(async (conn) =>
            {
                string query = $"UPDATE {TableName} SET {field}=@Value WHERE {result.Query}";
                result.Args.Add("Value", value);
                return await conn.ExecuteAsync(query, result.Args);
            });
        }
    }
}

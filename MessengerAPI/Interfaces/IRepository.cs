namespace MessengerAPI.Interfaces
{
    public interface IRepository<TEntity>
    {
        public Task CreateAsync(IDictionary<string, object> entity);
        public Task DeleteAsync(Guid id);
        public Task<TEntity?> GetAsync(Guid id);
        public Task UpdateAsync(Guid id, string field, object value);
        public IDictionary<string, object> EntityToDictionary(TEntity entity);
    }
}

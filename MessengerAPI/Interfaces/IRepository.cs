namespace MessengerAPI.Interfaces
{
    public interface IRepository<TEntity>
    {
        public Task CreateAsync(TEntity entity);
        public Task DeleteAsync(Guid id);
        public Task<TEntity?> GetAsync(Guid id);
    }
}

namespace MessengerAPI.Interfaces
{
    public interface IRepository<TEntity>
    {
        public abstract Task CreateAsync(TEntity entity);
        public abstract Task DeleteAsync(Guid id);
        public abstract Task<TEntity> GetAsync(Guid id);
    }
}

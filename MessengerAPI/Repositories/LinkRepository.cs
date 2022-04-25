using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;
using Dapper;

namespace MessengerAPI.Repositories
{
    public class LinkRepository : BaseRepository<Link>, ILinkRepository
    {
        public LinkRepository(IOptions<Connections> options) : base(options)
        {
        }

        public async override Task CreateAsync(Link entity)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO links (id, groupid, datecreate, onetime, isactive, isdeleted) " +
                    "VALUES(@Id, GroupId, DateCreate, OneTime, IsActive, IsDeleted)", entity);
            });
        }

        public async override Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE links SET isdeleted=true WHERE id=@Id AND isdeleted=false", id);
            });
        }

        public async override Task<Link?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Link>("SELECT * FROM links WHERE id=@Id", id);
            });
        }

        public async Task UpdateAsync(Guid id, bool isActive)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE links SET isactive=@IsActive WHERE id=@Id AND isdeleted=false", new { Id = id, IsActive = isActive });
            });
        }
    }
}

using MessengerAPI.Interfaces;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;
using Dapper;

namespace MessengerAPI.Repositories
{
    public class FileRepository : BaseRepository<Models.File>, IFileRepository
    {
        public FileRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(Models.File file)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO files (id, server, path, isdeleted) " +
                    "VALUES(@Id, @Server, @Path, @IsDeleted)", file);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE files SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<Models.File?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Models.File?>("SELECT * FROM files WHERE id=@Id AND isdeleted=false", new { id });
            });
        }
    }
}

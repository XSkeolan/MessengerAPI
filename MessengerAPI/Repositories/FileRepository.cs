using MessengerAPI.Interfaces;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class FileRepository : BaseRepository<Models.File>, IFileRepository
    {
        public FileRepository(IOptions<Connections> options) : base(options) { }

    }
}

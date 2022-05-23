using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class MessageFileRepository : BaseRepository<MessageFile>, IMessageFileRepository
    {
        public MessageFileRepository(IOptions<Connections> options) : base(options) { }
    }
}

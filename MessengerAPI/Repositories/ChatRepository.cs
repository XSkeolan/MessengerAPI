using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using Dapper;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(IOptions<Connections> options, IServiceContext serviceContext) : base(options, serviceContext) { }
    }
}

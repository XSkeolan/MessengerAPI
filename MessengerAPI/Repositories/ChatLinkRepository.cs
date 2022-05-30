using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ChatLinkRepository : BaseRepository<ChatLink>, IChannelLinkRepository
    {
        public ChatLinkRepository(IOptions<Connections> options) : base(options)
        {
        }
    }
}

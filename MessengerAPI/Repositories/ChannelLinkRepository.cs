using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ChannelLinkRepository : BaseRepository<ChannelLink>, IChannelLinkRepository
    {
        public ChannelLinkRepository(IOptions<Connections> options) : base(options)
        {
        }
    }
}

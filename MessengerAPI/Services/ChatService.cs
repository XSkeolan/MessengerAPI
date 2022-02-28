using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chats;

        public ChatService(IChatRepository chats)
        {
            _chats = chats;
        }

        public async Task CreateChat(Chat chat)
        {
            await _chats.CreateAsync(chat);
        }
    }
}

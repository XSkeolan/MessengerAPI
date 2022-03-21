using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        public Task CreateChat(Chat chat);
        public Task<IEnumerable<UserResponse>> InviteUsersAsync(Guid chatId, IEnumerable<Guid> users);
        public Task<ChatResponse?> GetChatAsync(Guid chatId);
    }
}
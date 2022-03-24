using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        Task CreateChatAsync(Chat chat);
        Task<IEnumerable<UserResponse>> InviteUsersAsync(Guid chatId, IEnumerable<Guid> users);
        Task<UserResponse> KickUsersAsync(Guid chatId, Guid userId);
        Task<ChatResponse?> GetChatAsync(Guid chatId);
        Task<ChatResponse?> EditNameAsync(Guid chatId, string name);
        Task DeleteChatAsync(Guid chatId);
        Task<ChatResponse?> EditDescriptionAsync(Guid chatId, string name);
        Task<IEnumerable<DialogInfoResponse>> GetDialogs(Guid? offset_id, int count);
        Task<bool> EditChatAdmin(Guid chatId, Guid userId, bool isAdmin);
    }
}
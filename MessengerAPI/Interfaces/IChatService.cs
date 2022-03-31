using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        Task<Guid> CreateChatAsync(Chat chat);
        Task<ShortUserResponse> InviteUserAsync(Guid chatId, Guid users);
        Task KickUserAsync(Guid chatId, Guid userId);
        Task<ChatResponse> GetChatAsync(Guid chatId);
        Task EditNameAsync(Guid chatId, string name);
        Task DeleteChatAsync(Guid chatId);
        Task EditDescriptionAsync(Guid chatId, string name);
        Task<IEnumerable<DialogInfoResponse>> GetDialogsAsync(Guid? offset_id, int count);
        Task<bool> ChatIsAvaliableAsync(Guid chatId);
        Task SetRole(Guid chatId, Guid userId, Guid roleId);
        Task<IEnumerable<RoleResponse>> GetRoles();
        Task DeleteMessageAsync(Guid chatId, Guid messagesId);
    }
}
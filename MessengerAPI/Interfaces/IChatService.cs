using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        Task CreateChatAsync(Chat chat);
        Task<ShortUserResponse> InviteUserAsync(Guid chatId, Guid users);
        Task KickUserAsync(Guid chatId, Guid userId);
        Task<ChatResponse> GetChatAsync(Guid chatId);
        Task EditNameAsync(Guid chatId, string name);
        Task DeleteChatAsync(Guid chatId);
        Task EditDescriptionAsync(Guid chatId, string name);
        Task<IEnumerable<DialogInfoResponse>> GetDialogsAsync(Guid? offset_id, int count);
        Task<bool> ChatIsAvaliableAsync(Guid chatId);
        Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId);
        Task<IEnumerable<RoleResponse>> GetRolesAsync();
        Task DeleteMessageAsync(Guid chatId, Guid messagesId);
        Task<IEnumerable<BaseUserResponse>> SearchUsersAsync(Guid chatId, string nickname);
        Task EditPhotoAsync(Guid chatId, Guid fileId);
    }
}
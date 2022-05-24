using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        Task CreateChatAsync(Chat chat);
        Task DeleteChatAsync(Guid chatId);
        Task<Chat> GetChatAsync(Guid chatId);

        Task<UserGroup> InviteUserAsync(Guid chatId, Guid users);
        Task KickUserAsync(Guid chatId, Guid userId);

        Task<IEnumerable<Chat>> GetDialogsAsync(Guid? offset_id, int count);

        Task EditNameAsync(Guid chatId, string name);
        Task EditPhotoAsync(Guid chatId, Guid fileId);

        Task<IEnumerable<UserType>> GetRolesAsync();
        Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId);

        Task<IEnumerable<User>> SearchUsersAsync(Guid chatId, string nickname);
    }
}
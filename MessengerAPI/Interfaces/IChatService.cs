using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        Task CreateChat(Chat chat, Guid[] inviteUsers);
    }
}
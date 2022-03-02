using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChatService
    {
        Task<ChatCreateResponse> CreateChat(Chat chat, Guid[] inviteUsers);
    }
}
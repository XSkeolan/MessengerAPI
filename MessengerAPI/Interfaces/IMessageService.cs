using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public interface IMessageService
    {
        Task<MessageResponse> SendMessageToUserAsync(Message message);
        Task<MessageResponse> SendMessageToChatAsync(Message message);
        Task<List<MessageResponse>> GetMessagesAsync(Guid companionId);
    }
}
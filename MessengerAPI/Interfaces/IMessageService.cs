using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public interface IMessageService
    {
        Task<MessageResponse> SendMessageAsync(Guid sessionId, Message message);
        Task<List<MessageResponse>> GetMessagesAsync(Guid sessionId, Guid companionId);
    }
}
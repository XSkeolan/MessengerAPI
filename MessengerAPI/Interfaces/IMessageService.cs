using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public interface IMessageService
    {
        Task<MessageResponse> SendMessageAsync(Message message);
        Task<List<MessageResponse>> GetMessagesAsync(Guid userId, IEnumerable<Guid> ids);
    }
}
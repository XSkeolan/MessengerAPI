using MessengerAPI.DTOs;
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IMessageService
    {
        Task<MessageResponse> SendMessageAsync(Message message);
    }
}
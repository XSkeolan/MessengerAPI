using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IMessageService
    {
        Task ChangePinStatusAsync(Guid messageId, bool status);
        Task DeleteMessageAsync(Guid messagesId);
        Task EditMessageAsync(Guid messageId, string newText);
        Task<IEnumerable<Message>> FindMessagesAsync(Guid chatId, string subtext);
        Task<IEnumerable<Message>> GetHistoryAsync(Guid chatId, DateTime dateStart, DateTime dateEnd);
        Task<Message> GetMessageAsync(Guid messageId);
        Task ReadMessageAsync(Guid messageId);
        Task SendMessageAsync(Message message);
        Task<Models.File> SendAttachment(Guid messageId, IFormFile file);
        Task<IEnumerable<byte[]>> GetMessageAttachments(Guid messageId);
        Task<Message?> GetLastMessageAsync(Guid chatId);
    }
}
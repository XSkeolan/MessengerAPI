using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MessengerAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;

        public MessageService(IMessageRepository messages, 
            IChatRepository chatRepository, 
            IUserChatRepository userChatRepository,
            IUserTypeRepository userTypeRepository,
            IServiceContext serviceContext)
        {
            _messageRepository = messages;
            _chatRepository = chatRepository;
            _userChatRepository = userChatRepository;
            _userTypeRepository = userTypeRepository;
            _serviceContext = serviceContext;
        }

        public async Task SendMessageAsync(Message message)
        {
            if (message.OriginalMessageId.HasValue)
            {
                if (await _messageRepository.GetAsync(message.OriginalMessageId.Value) == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }
            }
            if (message.ReplyMessageId.HasValue)
            {
                Message? replyMessage = await _messageRepository.GetAsync(message.ReplyMessageId.Value);
                if (replyMessage == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }

                message.Destination = replyMessage.Destination;
            }

            if (await _chatRepository.GetAsync(message.Destination) == null)
            {
                throw new ArgumentException(ResponseErrors.DESTINATION_NOT_FOUND);
            }

            if(await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId) == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }
            
            // true - нет вложений
            if (string.IsNullOrWhiteSpace(message.Text) && true && !message.OriginalMessageId.HasValue)
            {
                throw new InvalidOperationException(ResponseErrors.EMPTY_MESSAGE);
            }

            message.FromWhom = _serviceContext.UserId;
            await _messageRepository.CreateAsync(_messageRepository.EntityToDictionary(message));
        }

        public async Task<Message> GetMessageAsync(Guid messageId)
        {
            Message? message = await _messageRepository.GetAsync(messageId);
            if (message == null)
            {
                throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
            }

            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId);
            return userGroup != null ? message : throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
        }

        public async Task DeleteMessageAsync(Guid messagesId)
        {
            Message? message = await GetMessageAsync(messagesId);
            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId);
            UserType? userType = await _userTypeRepository.GetAsync(userGroup.UserTypeId);
            if (!userType.Permissions.Contains(Permissions.DELETE_MESSAGE) && message.FromWhom != _serviceContext.UserId)
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _messageRepository.DeleteAsync(messagesId);
        }

        public async Task ChangePinStatusAsync(Guid messageId, bool status)
        {
            Message? message = await GetMessageAsync(messageId);
            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId);
            UserType? userType = await _userTypeRepository.GetAsync(userGroup.UserTypeId);
            if (!userType.Permissions.Contains(Permissions.PIN_MESSAGE))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _messageRepository.UpdateAsync(messageId, typeof(Message).GetProperty("IsPinned").GetCustomAttribute<ColumnAttribute>().Name, status);
        }

        public async Task<IEnumerable<Message>> GetHistoryAsync(Guid chatId, DateTime? dateStart, DateTime? dateEnd)
        {
            if(!(dateStart.HasValue && dateEnd.HasValue))
            {
                throw new ArgumentException("Должны быть заполнены оба параметра");
            }

            Chat? chat = await _chatRepository.GetAsync(chatId);
            if(chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            IEnumerable<Message> chatMessages = await _messageRepository.GetMessagesByDestination(chatId);
            if(dateStart.HasValue)
            {
                chatMessages.Where(msg => msg.DateSend >= dateStart && msg.DateSend <= dateEnd);
            }
            return chatMessages;
        }

        public async Task DeleteHistoryAsync(Guid chatId, Guid? maxMessageId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            IEnumerable<Message> messageToDelete = await _messageRepository.GetMessagesByDestination(chatId);
            if (maxMessageId.HasValue)
            {
                messageToDelete = messageToDelete.OrderBy(msg => msg.DateSend).TakeWhile(msg => msg.Id != maxMessageId);
            }

            foreach (Message message in messageToDelete)
            {
                await _messageRepository.DeleteAsync(message.Id);
            }
        }

        public async Task EditMessageAsync(Guid messageId, string newText)
        {
            Message message = await GetMessageAsync(messageId);
            if(message.FromWhom != _serviceContext.UserId)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_SENDER);
            }

            await _messageRepository.UpdateAsync(messageId, "text", newText);
        }

        public async Task ReadMessageAsync(Guid messageId)
        {
            await _messageRepository.UpdateAsync(messageId, "isread", true);
        }

        public async Task<IEnumerable<Message>> FindMessagesAsync(Guid chatId, string subtext)
        {
            return (await _messageRepository.GetMessagesByDestination(chatId)).Where(x=> x.Text.Contains(subtext));
        }

        public async Task<Message?> GetLastMessageAsync(Guid chatId)
        {
            if(await _chatRepository.GetAsync(chatId) == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }
            return await _messageRepository.GetLastMessage(chatId);
        }
    }
}

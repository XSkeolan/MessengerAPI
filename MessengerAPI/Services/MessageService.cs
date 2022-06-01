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
        private readonly IFileRepository _fileRepository;
        private readonly IMessageFileRepository _messageFileRepository;
        private readonly IServiceContext _serviceContext;

        public MessageService(IMessageRepository messages,
            IChatRepository chatRepository,
            IUserChatRepository userChatRepository,
            IUserTypeRepository userTypeRepository,
            IFileRepository fileRepository,
            IMessageFileRepository messageFileRepository,
            IServiceContext serviceContext)
        {
            _messageRepository = messages;
            _chatRepository = chatRepository;
            _userChatRepository = userChatRepository;
            _userTypeRepository = userTypeRepository;
            _fileRepository = fileRepository;
            _messageFileRepository = messageFileRepository;
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

            if (await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId) == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            if (string.IsNullOrWhiteSpace(message.Text) && !message.OriginalMessageId.HasValue)
            {
                throw new InvalidOperationException(ResponseErrors.EMPTY_MESSAGE);
            }

            message.FromWhom = _serviceContext.UserId;
            await _messageRepository.CreateAsync(_messageRepository.EntityToDictionary(message));
        }

        public async Task<Models.File> SendAttachment(Guid messageId, IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException(ResponseErrors.FILE_IS_EMPTY);
            }

            var filePath = Path.Combine("D:\\Image", Path.GetRandomFileName());

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            Models.File newFile = new Models.File
            {
                Server = "http://localhost:5037/",
                Path = filePath
            };

            await _fileRepository.CreateAsync(_fileRepository.EntityToDictionary(newFile));
            MessageFile messageFile = new MessageFile
            {
                MessageId = messageId,
                FileId = newFile.Id
            };
            await _messageFileRepository.CreateAsync(_messageFileRepository.EntityToDictionary(messageFile));
            return newFile;
        }

        public async Task<IEnumerable<byte[]>> GetMessageAttachments(Guid messageId)
        {
            List<Guid> filesId = new List<Guid>((await _messageFileRepository.GetMessageFiles(messageId)).Select(x => x.FileId));
            List<byte[]> files = new List<byte[]>();
            foreach (Guid fileId in filesId)
            {
                Models.File file = await _fileRepository.GetAsync(fileId);
                using (var memoryStream = new MemoryStream())
                {
                    System.IO.File.OpenRead(file.Path).CopyTo(memoryStream);
                    files.Add(memoryStream.ToArray());
                }
            }
            return files;
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

        public async Task<IEnumerable<Message>> GetHistoryAsync(Guid chatId, DateTime dateStart, DateTime dateEnd)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            IEnumerable<Message> chatMessages = (await _messageRepository.GetMessagesByDestination(chatId))
                .Where(msg => msg.DateSend >= dateStart && msg.DateSend <= dateEnd);

            return chatMessages;
        }

        public async Task EditMessageAsync(Guid messageId, string newText)
        {
            Message message = await GetMessageAsync(messageId);
            if (message.FromWhom != _serviceContext.UserId)
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
            return (await _messageRepository.GetMessagesByDestination(chatId)).Where(x => x.Text.Contains(subtext));
        }

        public async Task<Message?> GetLastMessageAsync(Guid chatId)
        {
            if (await _chatRepository.GetAsync(chatId) == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }
            return await _messageRepository.GetLastMessage(chatId);
        }
    }
}

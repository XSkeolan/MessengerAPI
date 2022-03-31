using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IServiceContext _serviceContext;

        public MessageService(IMessageRepository messages, 
            IUserRepository userRepository, 
            IChatRepository chatRepository, 
            IUserChatRepository userChatRepository,
            IUserTypeRepository userTypeRepository,
            IServiceContext serviceContext)
        {
            _messageRepository = messages;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _userChatRepository = userChatRepository;
            _userTypeRepository = userTypeRepository;
            _serviceContext = serviceContext;
        }

        public async Task<MessageResponse> SendMessageAsync(Message message)
        {
            if (message.OriginalMessageId != null)
            {
                if (await _messageRepository.GetAsync(message.OriginalMessageId.Value) == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }
            }
            if(message.ReplyMessageId != null)
            {
                if (await _messageRepository.GetAsync(message.ReplyMessageId.Value) == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }
            }
            if(await _chatRepository.GetAsync(message.Destination) == null)
            {
                throw new ArgumentException(ResponseErrors.DESTINATION_NOT_FOUND);
            }

            message.From = _serviceContext.UserId;

            await _messageRepository.CreateAsync(message);

            return new MessageResponse 
            { 
                MessageId = message.Id, 
                FromId = message.From, 
                Message = message.Text, 
                Date = message.DateSend, 
                DestinationId = message.Destination,
                IsPinned = message.IsPinned,
                IsDeleted = message.IsDeleted
            };
        }

        public async Task ChangePinStatus(Guid messageId, bool status)
        {
            Message? message = await _messageRepository.GetAsync(messageId);
            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId);
            UserType? userType = await _userTypeRepository.GetAsync(userGroup.UserTypeId);
            if (!userType.Permissions.Contains(Permissions.PIN_MESSAGE))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _messageRepository.UpdateAsync(messageId, status);
        }

        public async Task<bool> MessageAvaliable(Guid messageId)
        {
            Message? message = await _messageRepository.GetAsync(messageId);
            if(message == null)
            {
                return false;
            }

            if(message.From == _serviceContext.UserId)
            {
                return true;
            }

            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(message.Destination, _serviceContext.UserId);
            return userGroup != null;
        }
    }
}

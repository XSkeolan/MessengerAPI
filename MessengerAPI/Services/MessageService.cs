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
        private readonly IServiceContext _serviceContext;

        public MessageService(IMessageRepository messages, IUserRepository userRepository, IChatRepository chatRepository, IUserChatRepository userChatRepository, IServiceContext serviceContext)
        {
            _messageRepository = messages;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _userChatRepository = userChatRepository;
            _serviceContext = serviceContext;
        }

        public async Task<MessageResponse> SendMessageAsync(Message message)
        {
            if (message.OriginalMessageId != null)
            {
                if (_messageRepository.GetAsync(message.OriginalMessageId ?? Guid.Empty) == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }
            }
            if(message.ReplyMessageId != null)
            {
                if (_messageRepository.GetAsync(message.ReplyMessageId ?? Guid.Empty) == null)
                {
                    throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
                }
            }

            await _messageRepository.CreateAsync(message);

            return new MessageResponse 
            { 
                MessageId = message.Id, 
                FromId = message.From, 
                Message = message.Text, 
                Date = message.DateSend, 
                DestinationId = message.Destination
            };
        }

        public async Task<List<MessageResponse>> GetMessagesAsync(Guid userId, IEnumerable<Guid> ids)
        {
            List<MessageResponse> responses = new List<MessageResponse>();
            //IEnumerable<Guid> chatsDestination = await _userChatRepository.GetUserChats(userId);
            //foreach (var id in ids.Distinct())
            //{
            //    Message message = await _messageRepository.GetAsync(id);
            //    if(message == null)
            //    {
            //        continue;
            //    }
                
            //    if(message.DestinationType == DestinationType.User)
            //    {
            //        if(message.Destination == userId || message.From == userId)
            //        {
            //            responses.Add(new MessageResponse { Message = message.Text, Date = message.DateSend, DestinationId = message.Destination, DestinationType=message.DestinationType, FromId = message.From, MessageId = message.Id });
            //        }
            //        else
            //        {
            //            throw new Exception(ResponseErrors.USER_HAS_NOT_ACCESS);
            //        }
            //    }
            //    else if(message.DestinationType == DestinationType.Chat)
            //    {
            //        if(chatsDestination.Count(x=> x==message.Destination) == 0)
            //        {
            //            throw new Exception(ResponseErrors.USER_HAS_NOT_ACCESS);
            //        }
            //        else
            //        {
            //            responses.Add(new MessageResponse { Message = message.Text, Date = message.DateSend, DestinationId = message.Destination, DestinationType = message.DestinationType, FromId = message.From, MessageId = message.Id });
            //        }
            //    }
            //}
            return responses;
        }
    }
}

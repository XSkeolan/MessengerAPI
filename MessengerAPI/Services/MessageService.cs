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

        public MessageService(IMessageRepository messages, IUserRepository userRepository, IChatRepository chatRepository)
        {
            _messageRepository = messages;
            _userRepository = userRepository;
            _chatRepository = chatRepository;
        }

        public async Task<MessageResponse> SendMessageAsync(Message message)
        {
            bool destinationExists = false;
            if (message.DestinationType == DestinationType.User)
            {
                destinationExists = await _userRepository.GetAsync(message.Destination) == null;
            }
            else if(message.DestinationType == DestinationType.Chat)
            {
                destinationExists = await _chatRepository.GetAsync(message.Destination) == null;
            }

            if (destinationExists || await _userRepository.GetAsync(message.From) == null)
            {
                throw new ArgumentException(ResponseErrors.DESTINATION_NOT_FOUND);
            }
            if (_messageRepository.GetAsync(message.OriginalMessageId ?? Guid.Empty) == null)
            {
                throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
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

        public async Task<List<MessageResponse>> GetMessagesAsync(Guid destinationId)
        {
            List<MessageResponse> responses = new List<MessageResponse>();
            IEnumerable<Message> messages = await _messageRepository.GetMessagesByDestination(destinationId);
            foreach (var message in messages)
            {
                responses.Add(new MessageResponse { Message = message.Text, Date = message.DateSend, DestinationId = message.Destination, FromId=message.From, MessageId = message.Id });
            }
            return responses;
        }
    }
}

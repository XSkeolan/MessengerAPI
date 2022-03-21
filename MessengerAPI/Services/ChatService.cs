using MessengerAPI.DTOs;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserChatRepository _userChatsRepository;
        private readonly IUserTypeRepository _userTypesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatTypeRepository _chatTypeRepository;
        private readonly IServiceContext _serviceContext;

        public ChatService(IChatRepository chats, IUserChatRepository users, IUserTypeRepository userTypes, IUserRepository userRepository, IChatTypeRepository chatTypeRepository, IServiceContext serviceContext)
        {
            _chatRepository = chats;
            _userChatsRepository = users;
            _userTypesRepository = userTypes;
            _userRepository = userRepository;
            _chatTypeRepository = chatTypeRepository;
            _serviceContext = serviceContext;
        }

        public async Task CreateChat(Chat chat)
        {
            await _chatRepository.CreateAsync(chat);
        }

        public async Task<IEnumerable<UserResponse>> InviteUsersAsync(Guid chatId, IEnumerable<Guid> users)
        {
            List<UserResponse> userResponses = new List<UserResponse>();

            Chat chat = await _chatRepository.GetAsync(chatId);
            ChatType chatType = await _chatTypeRepository.GetAsync(chat.Type);

            string userType;
            foreach (Guid id in users)
            {
                if (chatType.Type == "HasAnAdministrator" && id == _serviceContext.UserId)
                {
                    userType = "admin";
                }
                else
                {
                    userType = "user";
                }

                await _userChatsRepository.CreateAsync(new UserGroup
                {
                    ChatId = chatId,
                    UserId = id,
                    UserTypeId = await _userTypesRepository.GetIdByTypeName(userType)
                });

                User user = await _userRepository.GetAsync(id);
                userResponses.Add(new UserResponse
                {
                    Id = id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname,
                    UserType = userType
                });
            }

            return userResponses;
        }

        public async Task<ChatResponse?> GetChatAsync(Guid chatId)
        {
            Chat chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                return null;
            }
            else
            {
                IEnumerable<Guid> usersId = await _userChatsRepository.GetChatUsers(chatId);

                if (!usersId.Contains(_serviceContext.UserId))
                    throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);

                ChatResponse response = new ChatResponse 
                { 
                    ChatId = chat.Id,
                    Name = chat.Name,
                    Description = chat.Description,
                    ChatType = chat.Type,
                    InviteUsers = new List<UserResponse>()
                };

                foreach (var id in usersId)
                {
                    User user = await _userRepository.GetAsync(id);
                    UserType usertype = await _userTypesRepository.GetUserTypeInChat(user.Id, chatId);

                    UserResponse userResponse = new UserResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Nickname = user.Nickname,
                        UserType = usertype.Type
                    };
                    response.InviteUsers = response.InviteUsers.Append(userResponse);
                }
                
                return response;
            }
        }
    }
}

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
            ChatType chatType = await _chatTypeRepository.GetAsync(chat.Type);

            if (chatType.Type == "HasAnAdministrator")
            {
                await _userChatsRepository.CreateAsync(new UserGroup 
                { 
                    ChatId = chat.Id, 
                    UserId = _serviceContext.UserId, 
                    UserTypeId = await _userTypesRepository.GetIdByTypeName("admin") 
                });
            }
        }

        public async Task<IEnumerable<UserResponse>> InviteUsersAsync(Guid chatId, IEnumerable<Guid> users)
        {
            IEnumerable<Guid> inviteUsers = users;
            List<UserResponse> userResponses = new List<UserResponse>();

            Chat chat = await _chatRepository.GetAsync(chatId);
            ChatType chatType = await _chatTypeRepository.GetAsync(chat.Type);

            if (chatType.Type == "HasAnAdministrator")
            {
                Guid admin = await _userChatsRepository.GetChatAdmin(chatId);
                User user = await _userRepository.GetAsync(admin);
                userResponses.Add(new UserResponse {
                    Id=admin,
                    Name=user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname,
                    UserType = "admin"
                });
                inviteUsers = inviteUsers.Where(x => x != admin);
            }

            foreach (Guid id in inviteUsers)
            {
                await _userChatsRepository.CreateAsync(new UserGroup
                {
                    ChatId = chatId,
                    UserId = id,
                    UserTypeId = await _userTypesRepository.GetIdByTypeName("user")
                });
                User user = await _userRepository.GetAsync(id);
                userResponses.Add(new UserResponse
                {
                    Id = id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname,
                    UserType = "user"
                });
            }
            return userResponses;
        }
    }
}

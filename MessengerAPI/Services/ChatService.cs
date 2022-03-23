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
        private readonly IServiceContext _serviceContext;

        public ChatService(IChatRepository chats, IUserChatRepository users, IUserTypeRepository userTypes, IUserRepository userRepository, IServiceContext serviceContext)
        {
            _chatRepository = chats;
            _userChatsRepository = users;
            _userTypesRepository = userTypes;
            _userRepository = userRepository;
            _serviceContext = serviceContext;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            await _chatRepository.CreateAsync(chat);
        }

        public async Task<IEnumerable<UserResponse>> InviteUsersAsync(Guid chatId, IEnumerable<Guid> users)
        {
            List<UserResponse> userResponses = new List<UserResponse>();

            Chat? chat = await _chatRepository.GetAsync(chatId);
            if(chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            string usertype = "admin";
            User? user;

            IEnumerable<Guid> userInChat = await _userChatsRepository.GetChatUsers(chatId);
            if (!userInChat.Any())
            {
                await _userChatsRepository.CreateAsync(new UserGroup
                {
                    ChatId = chatId,
                    UserId = _serviceContext.UserId,
                    UserTypeId = await _userTypesRepository.GetIdByTypeName(usertype)
                });

                user = await _userRepository.GetAsync(_serviceContext.UserId);
                userResponses.Add(new UserResponse
                {
                    Id = _serviceContext.UserId,
                    Name = user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname,
                    UserType = "admin"
                });

                if (users.Count() > 1)
                {
                    usertype = "user";
                }
                else
                {
                    throw new ArgumentException(ResponseErrors.INVITE_USERS_LIST_EMPTY);
                }
            }
            else
            {
                usertype = "user";
            }

            foreach (Guid userId in users)
            {
                await _userChatsRepository.CreateAsync(new UserGroup
                {
                    ChatId = chatId,
                    UserId = userId,
                    UserTypeId = await _userTypesRepository.GetIdByTypeName(usertype)
                });

                user = await _userRepository.GetAsync(userId);
                userResponses.Add(new UserResponse
                {
                    Id = userId,
                    Name = user.Name,
                    Surname = user.Surname,
                    Nickname = user.Nickname,
                    UserType = usertype
                });
            }

            return userResponses;
        }

        public async Task<ChatResponse?> GetChatAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Name = chat.Name,
                Description = chat.Description,
            };

            return response;

        }

        public async Task<ChatResponse?> EditNameAsync(Guid chatId, string name)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);

            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }
            if (chat.Name == name)
            {
                return null;
            }

            await _chatRepository.UpdateAsync(chatId, name, chat.Description);

            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Name = name,
                Description = chat.Description
            };

            return response;
        }

        public async Task<ChatResponse?> EditDescriptionAsync(Guid chatId, string description)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);

            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }
            if (chat.Description == description)
            {
                return null;
            }

            await _chatRepository.UpdateAsync(chatId, chat.Name, description);

            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Name = chat.Name,
                Description = description
            };

            return response;
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);

            if(chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            UserType usertype = await _userTypesRepository.GetUserTypeInChat(_serviceContext.UserId, chatId);
            if (usertype.Type != "admin")
            {
                throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
            }

            IEnumerable<Guid> admins = await _userChatsRepository.GetChatAdmins(chatId);
            IEnumerable<Guid> users = await _userChatsRepository.GetChatUsers(chatId);
            if(!admins.Except(users).Any() && !users.Except(admins).Any() && users.Count()==2) // не идеальное условие (могли пригласить нескольких пользователей а потом кикнуть)
            {
                throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
            }
            await _chatRepository.DeleteAsync(chatId);
        }

        public async Task<IEnumerable<DialogInfoResponse>> GetDialogs(Guid? offset_id, int count)
        {
            List<DialogInfoResponse> responses = new List<DialogInfoResponse>();

            IEnumerable<Guid> chats = await _userChatsRepository.GetUserChats(_serviceContext.UserId);
            if (offset_id != null)
            {
                chats = chats.SkipWhile(x => x != offset_id).Skip(1);
            }
            chats = chats.Take(count);

            foreach (Guid chatId in chats)
            {
                Chat? chat = await _chatRepository.GetAsync(chatId);
                Message? message = await _chatRepository.GetLastMessage(chatId);
                if (message == null)
                {
                    responses.Add(new DialogInfoResponse { Id = chat.Id, Name = chat.Name, Photo = 1, LastMessageDateSend = null, LastMessageText = null });
                }
                else
                {
                    responses.Add(new DialogInfoResponse { Id = chat.Id, Name = chat.Name, Photo = 1, LastMessageDateSend = message.DateSend, LastMessageText = message.Text });
                }
            }

            return responses;
        }
    }
}

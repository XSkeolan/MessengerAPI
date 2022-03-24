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
            chat.Creator = _serviceContext.UserId;
            await _chatRepository.CreateAsync(chat);
        }

        private async Task<bool> IsAdmin(Guid chatId, Guid userId)
        {
            IEnumerable<Guid> users = await _userChatsRepository.GetChatAdmins(chatId);
            return users.Contains(userId);
        }

        public async Task<IEnumerable<UserResponse>> InviteUsersAsync(Guid chatId, IEnumerable<Guid> users)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if(chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            users = users.Distinct();
            string usertype = "admin";
            List<UserResponse> userResponses = new List<UserResponse>();
            IEnumerable<Guid> usersInChat = await _userChatsRepository.GetChatUsers(chatId);

            if (!usersInChat.Any())
            {
                if(_serviceContext.UserId!=chat.Creator)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
                }

                int count = users.Count();
                if (count < 2)
                {
                    throw new ArgumentException("Нужно пригласить минимум 2 человека");
                }
                else if(count > 2)
                {
                    userResponses.Add(await CreateUserInChat(chatId, _serviceContext.UserId, usertype));

                    users = users.Where(x=>x!=_serviceContext.UserId);
                    usertype = "user";
                }
            }
            else
            {
                if(!await IsAdmin(chatId, _serviceContext.UserId))
                {
                    throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
                }
                if(!users.Any())
                {
                    throw new ArgumentException(ResponseErrors.INVITE_USERS_LIST_EMPTY);
                }
                usertype = "user";
            }

            foreach (Guid userId in users)
            {
                userResponses.Add(await CreateUserInChat(chatId, userId, usertype));
            }

            return userResponses;
        }

        private async Task<UserResponse> CreateUserInChat(Guid chatId, Guid userId, string userType)
        {
            await _userChatsRepository.CreateAsync(new UserGroup
            {
                ChatId = chatId,
                UserId = userId,
                UserTypeId = await _userTypesRepository.GetIdByTypeName(userType)
            });

            User? user = await _userRepository.GetAsync(_serviceContext.UserId);
            return new UserResponse
            {
                Id = _serviceContext.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Nickname = user.Nickname,
                UserType = userType
            };
        }

        public async Task<UserResponse> KickUsersAsync(Guid chatId, Guid userId)
        {
            UserResponse userResponse;
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            if (!await IsAdmin(chatId, _serviceContext.UserId))
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_ADMIN_REQUIRED);
            }

            UserGroup? group = await _userChatsRepository.GetByChatAndUserAsync(chatId, userId);
            if (group == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }
            else
            {
                // добавить проверку на личный чат
                UserType? userType = await _userTypesRepository.GetAsync(group.UserTypeId);
                if (userType.Type == "admin" && _serviceContext.UserId != chat.Creator)
                {
                    throw new InvalidOperationException(ResponseErrors.CHAT_ADMIN_NOT_DELETED);
                }

                await _userChatsRepository.DeleteAsync(group.Id);

                User? user = await _userRepository.GetAsync(group.UserId);
                userResponse = new UserResponse { Id = user.Id, Name=user.Name, Nickname = user.Nickname, Surname = user.Surname, UserType=userType.Type };
            }

            return userResponse;
        }

        public async Task<bool> EditChatAdmin(Guid chatId, Guid userId, bool isAdmin)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            if (!await IsAdmin(chatId, _serviceContext.UserId))
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_ADMIN_REQUIRED);
            }

            UserGroup? userGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, userId);
            if(userGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            //if() это не личный чата
            if (await IsAdmin(chatId, userId))
                return false;

            Guid userType;
            if (isAdmin)
            {
                userType = await _userTypesRepository.GetIdByTypeName("admin");
            }
            else
            {
                userType = await _userTypesRepository.GetIdByTypeName("user");
            }

            await _userChatsRepository.UpdateAsync(userGroup.Id, userType);

            return true;
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
            
            if (!await IsAdmin(chatId, _serviceContext.UserId))
            {
                throw new InvalidOperationException(ResponseErrors.USER_HAS_NOT_ACCESS);
            }

            IEnumerable<Guid> admins = await _userChatsRepository.GetChatAdmins(chatId);
            IEnumerable<Guid> users = await _userChatsRepository.GetChatUsers(chatId);
            if(!admins.Except(users).Any() && !users.Except(admins).Any() && users.Count()==2) // не идеальное условие (могли пригласить нескольких пользователей а потом кикнуть)
                // либо ориентироваться на isDeleted либо новая таблица в бд
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

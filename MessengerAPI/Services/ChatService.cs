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
        private readonly IFileRepository _fileRepository;

        public ChatService(IChatRepository chats, IUserChatRepository users, IUserTypeRepository userTypes, IUserRepository userRepository, IFileRepository fileRepository, IServiceContext serviceContext)
        {
            _chatRepository = chats;
            _userChatsRepository = users;
            _userTypesRepository = userTypes;
            _userRepository = userRepository;
            _serviceContext = serviceContext;
            _fileRepository = fileRepository;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            chat.CreatorId = _serviceContext.UserId;
            await _chatRepository.CreateAsync(_chatRepository.EntityToDictionary(chat));
        }

        private async Task<bool> CurrentUserHaveRights(Guid chatId, string right, Guid? userId=null)
        {
            UserGroup? currentUserGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            UserType? currentUserRole = await _userTypesRepository.GetAsync(currentUserGroup.UserTypeId);

            if (userId.HasValue)
            {
                UserGroup? userGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, userId.Value);
                if (userGroup == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
                }

                UserType? userType = await _userTypesRepository.GetAsync(userGroup.UserTypeId);
                return currentUserRole.Permissions.Contains(right) && currentUserRole.PriorityLevel <= userType.PriorityLevel;
            }
            else
            {
                return currentUserRole.Permissions.Contains(right) || (await _userChatsRepository.GetChatUsers(chatId)).Count() == 1;
            }
        }

        public async Task<UserGroup> InviteUserAsync(Guid chatId, Guid userId)
        {
            await GetChatAsync(chatId);

            User? user = await _userRepository.GetAsync(userId);
            if(user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            
            int countUsers = (await _userChatsRepository.GetChatUsers(chatId)).Count();

            if (countUsers > 0 && !(await CurrentUserHaveRights(chatId, Permissions.INVITE_USER)))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            if ((await _userChatsRepository.GetChatUsers(chatId)).Contains(userId))
            {
                throw new ArgumentException(ResponseErrors.USER_ALREADY_IN_CHAT);
            }

            UserType userType = await _userTypesRepository.GetDefaultType();
            UserGroup userGroup = new UserGroup
            {
                GroupId = chatId,
                UserId = userId,
                UserTypeId = userType.Id,
            };

            await _userChatsRepository.CreateAsync(_userChatsRepository.EntityToDictionary(userGroup));

            return userGroup;
        }

        public async Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId)
        {
            await GetChatAsync(chatId);

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            UserGroup? userGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, userId);
            if (userGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? userType = await _userTypesRepository.GetAsync(userGroup.UserTypeId);
            UserGroup currentUser = await _userChatsRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            UserType currnetUserType = await _userTypesRepository.GetAsync(currentUser.UserTypeId);

            if(currnetUserType.PriorityLevel <= userType.PriorityLevel && (await _userChatsRepository.GetUserChatsAsync(chatId)).Any())
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            UserType? newRole = await _userTypesRepository.GetAsync(roleId);
            if (newRole == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_ROLE_NOT_FOUND);
            }

            if(!await CurrentUserHaveRights(chatId, Permissions.EDIT_PERMISSION))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _userChatsRepository.UpdateAsync(userGroup.Id, "usertypeid", newRole.Id);

            return;
        }

        public async Task KickUserAsync(Guid chatId, Guid userId)
        {
            await GetChatAsync(chatId);

            if (!await CurrentUserHaveRights(chatId, Permissions.KICK_USER))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            UserGroup? kickUserGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, userId);
            if(kickUserGroup == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? kickUserType = await _userTypesRepository.GetAsync(kickUserGroup.UserTypeId);
            UserGroup? currentUserGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            UserType? currentUserType = await _userTypesRepository.GetAsync(currentUserGroup.UserTypeId);

            if(currentUserType.PriorityLevel <= kickUserType.PriorityLevel)
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            if (kickUserType?.Id == currentUserType?.Id)
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_MODER_NOT_DELETED);
            }

            await _userChatsRepository.DeleteAsync(kickUserGroup.Id);
        }

        public async Task<Chat> GetChatAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if(chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            chat.CountUser = (await _userChatsRepository.GetChatUsers(chatId)).Count();
            return chat;
        }

        public async Task EditNameAsync(Guid chatId, string name)
        {
            await GetChatAsync(chatId);

            if(!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.UpdateAsync(chatId, "name", name);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            await GetChatAsync(chatId);

            if (!await CurrentUserHaveRights(chatId, Permissions.DELETE_CHAT))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            IEnumerable<Guid> chatUsers = await _userChatsRepository.GetChatUsers(chatId);
            foreach (Guid userId in chatUsers)
            {
                UserGroup? userGroup = await _userChatsRepository.GetByChatAndUserAsync(chatId, userId);
                await _userChatsRepository.DeleteAsync(userGroup.Id);
            }

            await _chatRepository.DeleteAsync(chatId);
        }

        public async Task<IEnumerable<Chat>> GetDialogsAsync(Guid? offset_id, int count)
        {
            IEnumerable<Guid> chats = await _userChatsRepository.GetUserChatsAsync(_serviceContext.UserId);
            if (offset_id.HasValue)
            {
                chats = chats.SkipWhile(x => x != offset_id).Skip(1);
            }

            chats = chats.Take(count);
            if(!chats.Any())
            {
                throw new ArgumentNullException(ResponseErrors.USER_LIST_CHATS_IS_EMPTY);
            }
            return await Task.WhenAll(chats.Select(async x => await _chatRepository.GetAsync(x)));
        }

        public async Task<IEnumerable<UserType>> GetRolesAsync()
        {
            return await _userTypesRepository.GetAll();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(Guid chatId, string nickname)
        {
            await GetChatAsync(chatId);

            IEnumerable<Guid> users = await _userChatsRepository.GetChatUsers(chatId);
            List<User> results = new List<User>();
            foreach (Guid user in users)
            {
                User? foundUser = await _userRepository.GetAsync(user);
                if (foundUser.Nickname.Contains(nickname))
                {
                    results.Add(foundUser);
                }
            }

            return results;
        }

        public async Task EditPhotoAsync(Guid chatId, Guid fileId)
        {
            await GetChatAsync(chatId);

            Models.File? file = await _fileRepository.GetAsync(fileId);
            if(file == null)
            {
                throw new ArgumentException(ResponseErrors.FILE_NOT_FOUND);
            }

            await _chatRepository.UpdateAsync(chatId, "photoid", fileId);
        }

        private async Task<byte[]> GetPhoto(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if(chat.PhotoId.HasValue)
            {
                Models.File photo = await _fileRepository.GetAsync(chat.PhotoId.Value);

                var client = new HttpClient();
                return await client.GetByteArrayAsync(photo.Server + photo.Path);
            }
            else
            {
                return Array.Empty<byte>();
            }
        }
    }
}

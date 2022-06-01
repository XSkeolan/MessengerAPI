using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MessengerAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserChatRepository _userChatRepository;
        private readonly IUserTypeRepository _userTypesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IServiceContext _serviceContext;
        private readonly IFileRepository _fileRepository;
        private readonly IChannelLinkRepository _channelLinkRepository;

        public ChatService(IChatRepository chats,
            IUserChatRepository users,
            IUserTypeRepository userTypes,
            IUserRepository userRepository,
            IFileRepository fileRepository,
            IChannelLinkRepository channelLinkRepository,
            IServiceContext serviceContext)
        {
            _chatRepository = chats;
            _userChatRepository = users;
            _userTypesRepository = userTypes;
            _userRepository = userRepository;
            _serviceContext = serviceContext;
            _fileRepository = fileRepository;
            _channelLinkRepository = channelLinkRepository;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            chat.CreatorId = _serviceContext.UserId;
            await _chatRepository.CreateAsync(_chatRepository.EntityToDictionary(chat));
        }

        private async Task<bool> CurrentUserHaveRights(Guid chatId, string right, Guid? userId = null)
        {
            UserGroup? currentUserGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUserGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? currentUserRole = await _userTypesRepository.GetAsync(currentUserGroup.UserTypeId);
            if (currentUserRole.Permissions == null && (await _userChatRepository.GetChatUsersAsync(chatId)).Count() > 1)
            {
                return false;
            }

            if (userId.HasValue)
            {
                UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId.Value);
                if (userGroup == null)
                {
                    throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
                }

                UserType? userType = await _userTypesRepository.GetAsync(userGroup.UserTypeId);
                return currentUserRole.Permissions.Contains(right) && currentUserRole.PriorityLevel <= userType.PriorityLevel;
            }
            else
            {
                return (await _userChatRepository.GetChatUsersAsync(chatId)).Count() == 1 || currentUserRole.Permissions.Contains(right);
            }
        }

        public async Task<UserGroup> InviteUserAsync(Guid chatId, Guid userId)
        {
            Chat chat = await GetChatAsync(chatId);

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            int countUsers = (await _userChatRepository.GetChatUsersAsync(chatId)).Count();

            if (countUsers > 0 && !(await CurrentUserHaveRights(chatId, Permissions.INVITE_USER)))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            if ((await _userChatRepository.GetChatUsersAsync(chatId)).Contains(userId))
            {
                throw new ArgumentException(ResponseErrors.USER_ALREADY_IN_CHAT);
            }

            UserType? userType = await _userTypesRepository.GetAsync(chat.DefaultUserTypeId);
            UserGroup userGroup = new UserGroup
            {
                GroupId = chatId,
                UserId = userId,
                UserTypeId = userType.Id,
            };

            await _userChatRepository.CreateAsync(_userChatRepository.EntityToDictionary(userGroup));

            return userGroup;
        }

        public async Task<UserGroup> JoinAsync(Guid chatId)
        {
            Chat chat = await GetChatAsync(chatId);
            if ((await _userTypesRepository.GetByTypeNameAsync("subscriber")).Id != chat.DefaultUserTypeId)
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_PRIVATE);
            }

            if ((await _userChatRepository.GetChatUsersAsync(chatId)).Contains(_serviceContext.UserId))
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_IN_CHANNEL);
            }

            UserGroup userChannel = new UserGroup
            {
                GroupId = chatId,
                UserId = _serviceContext.UserId,
                UserTypeId = chat.DefaultUserTypeId
            };

            await _userChatRepository.CreateAsync(_userChatRepository.EntityToDictionary(userChannel));

            return userChannel;
        }

        public async Task LeaveAsync(Guid chatId)
        {
            await GetChatAsync(chatId);

            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (userGroup == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            await _userChatRepository.DeleteAsync(userGroup.Id);
        }

        public async Task<Chat> JoinByLinkAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(ResponseErrors.INVALID_FIELDS);
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            Guid channelLinkId = Guid.Parse(tokenS.Claims.First(claim => claim.Type == ClaimsIdentity.DefaultNameClaimType).Value);
            DateTime dateEnd = DateTime.Parse(tokenS.Claims.First(claim => claim.Type == "DateEnd").Value);

            ChatLink? channelLink = await _channelLinkRepository.GetAsync(channelLinkId);
            if (channelLink == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_ALREADY_USED);
            }

            if (channelLink.DateEnd < DateTime.UtcNow.ToLocalTime())
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_INVALID);
            }

            if (channelLink.IsOneTime)
            {
                await _channelLinkRepository.DeleteAsync(channelLinkId);
            }

            Chat? chat = await _chatRepository.GetAsync(channelLink.GroupId);
            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(chat.Id, _serviceContext.UserId);
            if (userGroup != null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_ALREADY_IN_CHAT);
            }

            userGroup = new UserGroup
            {
                GroupId = channelLink.GroupId,
                UserId = _serviceContext.UserId,
                UserTypeId = chat.DefaultUserTypeId
            };

            await _userChatRepository.CreateAsync(_userChatRepository.EntityToDictionary(userGroup));

            return chat;
        }

        public async Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId)
        {
            await GetChatAsync(chatId);

            User? user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
            if (userGroup == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? userType = await _userTypesRepository.GetAsync(userGroup.UserTypeId);
            UserGroup? currentUser = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            if (currentUser == null)
            {
                throw new InvalidOperationException(ResponseErrors.USER_NOT_PARTICIPANT);
            }
            UserType? currnetUserType = await _userTypesRepository.GetAsync(currentUser.UserTypeId);

            if (currnetUserType.PriorityLevel <= userType.PriorityLevel && (await _userChatRepository.GetUserChatsAsync(chatId)).Any())
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            UserType? newRole = await _userTypesRepository.GetAsync(roleId);
            if (newRole == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_ROLE_NOT_FOUND);
            }

            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_PERMISSION))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _userChatRepository.UpdateAsync(userGroup.Id, "usertypeid", newRole.Id);

            return;
        }

        public async Task KickUserAsync(Guid chatId, Guid userId)
        {
            await GetChatAsync(chatId);

            if (!await CurrentUserHaveRights(chatId, Permissions.KICK_USER))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            UserGroup? kickUserGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
            if (kickUserGroup == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
            }

            UserType? kickUserType = await _userTypesRepository.GetAsync(kickUserGroup.UserTypeId);
            UserGroup? currentUserGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, _serviceContext.UserId);
            UserType? currentUserType = await _userTypesRepository.GetAsync(currentUserGroup.UserTypeId);

            if (currentUserType.PriorityLevel <= kickUserType.PriorityLevel)
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_ROLE_FOR_OPENATION);
            }

            if (kickUserType?.Id == currentUserType?.Id)
            {
                throw new InvalidOperationException(ResponseErrors.CHAT_MODER_NOT_DELETED);
            }

            await _userChatRepository.DeleteAsync(kickUserGroup.Id);
        }

        public async Task<Chat> GetChatAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                throw new ArgumentException(ResponseErrors.CHAT_NOT_FOUND);
            }

            chat.CountUser = (await _userChatRepository.GetChatUsersAsync(chatId)).Count();
            return chat;
        }

        public async Task<IEnumerable<Chat>> FindChatByName(string chatName)
        {
            IEnumerable<Chat> chats = await _chatRepository.GetChatsByNameAsync(chatName);
            UserType userType = await _userTypesRepository.GetByTypeNameAsync("subscriber");
            return chats.Where(chat => chat.DefaultUserTypeId == userType.Id);
        }

        public async Task EditNameAsync(Guid chatId, string name)
        {
            await GetChatAsync(chatId);

            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.UpdateAsync(chatId, "name", name);
        }

        public async Task EditDescriptionAsync(Guid chatId, string newDescription)
        {
            await GetChatAsync(chatId);

            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.UpdateAsync(chatId, "description", newDescription);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            await GetChatAsync(chatId);

            if (!await CurrentUserHaveRights(chatId, Permissions.DELETE_CHAT))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            IEnumerable<Guid> chatUsers = await _userChatRepository.GetChatUsersAsync(chatId);
            foreach (Guid userId in chatUsers)
            {
                UserGroup? userGroup = await _userChatRepository.GetByChatAndUserAsync(chatId, userId);
                await _userChatRepository.DeleteAsync(userGroup.Id);
            }

            await _chatRepository.DeleteAsync(chatId);
        }

        public async Task CreateInvitationLinkAsync(ChatLink channelLink)
        {
            await GetChatAsync(channelLink.GroupId);
            if (!await CurrentUserHaveRights(channelLink.GroupId, Permissions.INVITE_LINK))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _channelLinkRepository.CreateAsync(_channelLinkRepository.EntityToDictionary(channelLink));
        }

        public async Task DeleteInvitationLinkAsync(Guid channelLinkId)
        {
            ChatLink? link = await _channelLinkRepository.GetAsync(channelLinkId);
            if (link == null)
            {
                throw new ArgumentException(ResponseErrors.CHANNEL_LINK_NOT_FOUND);
            }

            if (!await CurrentUserHaveRights(link.GroupId, Permissions.INVITE_LINK))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _channelLinkRepository.DeleteAsync(channelLinkId);
        }

        public async Task ChangeCreatorAsync(Guid chatlId, Guid userId)
        {
            await GetChatAsync(chatlId);
            if (!await CurrentUserHaveRights(chatlId, Permissions.CHANGE_CREATOR))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            User? user = await _userRepository.GetAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }
            else
            {
                if (!(await _userChatRepository.GetChatUsersAsync(chatlId)).Contains(userId))
                {
                    throw new ArgumentException(ResponseErrors.USER_NOT_PARTICIPANT);
                }
            }

            UserGroup? currentAdmin = await _userChatRepository.GetByChatAndUserAsync(chatlId, _serviceContext.UserId);
            UserGroup? newAdmin = await _userChatRepository.GetByChatAndUserAsync(chatlId, userId);

            if (newAdmin == null)
            {
                throw new ArgumentException(ResponseErrors.USER_NOT_FOUND);
            }

            await _userChatRepository.UpdateAsync(currentAdmin.Id, "usertypeid", (await _userTypesRepository.GetByTypeNameAsync("moderator")).Id);
            await _chatRepository.UpdateAsync(chatlId, "creatorid", userId);
            await _userChatRepository.UpdateAsync(newAdmin.Id, "usertypeid", currentAdmin.UserTypeId);
        }

        public async Task<IEnumerable<Chat>> GetDialogsAsync(Guid? offset_id, int count)
        {
            IEnumerable<Guid> chats = await _userChatRepository.GetUserChatsAsync(_serviceContext.UserId);
            if (offset_id.HasValue)
            {
                chats = chats.SkipWhile(x => x != offset_id).Skip(1);
            }

            chats = chats.Take(count);
            if (!chats.Any())
            {
                throw new ArgumentException(ResponseErrors.USER_LIST_CHATS_IS_EMPTY);
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

            IEnumerable<Guid> users = await _userChatRepository.GetChatUsersAsync(chatId);
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
            if (file == null)
            {
                throw new ArgumentException(ResponseErrors.FILE_NOT_FOUND);
            }

            await _chatRepository.UpdateAsync(chatId, "photoid", fileId);
        }

        private async Task<byte[]> GetPhoto(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat.PhotoId.HasValue)
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

        public async Task<UserType> GetAdminRoleAsync()
        {
            return await _userTypesRepository.GetByMaxPriorityAsync();
        }
    }
}

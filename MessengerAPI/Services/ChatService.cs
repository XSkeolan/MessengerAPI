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
        private readonly IMessageRepository _messageRepository;
        private readonly IFileRepository _fileRepository;

        public ChatService(IChatRepository chats, IUserChatRepository users, IUserTypeRepository userTypes, IUserRepository userRepository, IMessageRepository messageRepository, IFileRepository fileRepository, IServiceContext serviceContext)
        {
            _chatRepository = chats;
            _userChatsRepository = users;
            _userTypesRepository = userTypes;
            _userRepository = userRepository;
            _serviceContext = serviceContext;
            _messageRepository = messageRepository;
            _fileRepository = fileRepository;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            chat.CreatorId = _serviceContext.UserId;
            await _chatRepository.CreateAsync(chat);
        }

        public async Task<bool> ChatIsAvaliableAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            if (chat == null)
            {
                return false;
            }

            if (!(await _userChatsRepository.GetChatUsers(chatId)).Contains(_serviceContext.UserId))
            {
                return false;
            }

            return true;
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

        public async Task<ShortUserResponse> InviteUserAsync(Guid chatId, Guid userId)
        {
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
                ChatId = chatId,
                UserId = userId,
                UserTypeId = userType.Id,
            };

            await _userChatsRepository.CreateAsync(userGroup);

            return new ShortUserResponse
            {
                UserTypeId = userType.Id,
                Name = user.Name,
                Surname = user.Surname,
                Nickname = user.Nickname
            };
        }

        public async Task SetRoleAsync(Guid chatId, Guid userId, Guid roleId)
        {
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
            await _userChatsRepository.UpdateAsync(userGroup.Id, newRole.Id);

            return;
        }

        public async Task KickUserAsync(Guid chatId, Guid userId)
        {
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

        public async Task<ChatResponse> GetChatAsync(Guid chatId)
        {
            Chat? chat = await _chatRepository.GetAsync(chatId);
            int count = (await _userChatsRepository.GetChatUsers(chatId)).Count();

            ChatResponse response = new ChatResponse
            {
                ChatId = chat.Id,
                Name = chat.Name,
                Description = chat.Description,
                CountUsers = count
            };

            return response;
        }

        public async Task EditNameAsync(Guid chatId, string name)
        {
            if(!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.UpdateAsync(chatId, name, (await _chatRepository.GetAsync(chatId)).Description);
        }

        public async Task EditDescriptionAsync(Guid chatId, string description)
        {
            if (!await CurrentUserHaveRights(chatId, Permissions.EDIT_CHAT_INFO))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.UpdateAsync(chatId, (await _chatRepository.GetAsync(chatId)).Name, description);
        }

        public async Task DeleteChatAsync(Guid chatId)
        {
            if (!await CurrentUserHaveRights(chatId, Permissions.DELETE_CHAT))
            {
                throw new InvalidOperationException(ResponseErrors.PERMISSION_DENIED);
            }

            await _chatRepository.DeleteAsync(chatId);
        }

        public async Task<IEnumerable<DialogInfoResponse>> GetDialogsAsync(Guid? offset_id, int count)
        {
            List<DialogInfoResponse> responses = new List<DialogInfoResponse>();
            IEnumerable<Guid> chats = await _userChatsRepository.GetUserChatsAsync(_serviceContext.UserId);

            if (offset_id.HasValue)
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
                    responses.Add(new DialogInfoResponse 
                    { 
                        Id = chat.Id, 
                        Name = chat.Name, 
                        Photo = 1, 
                        LastMessageDateSend = null, 
                        LastMessageText = null 
                    });
                }
                else
                {
                    responses.Add(new DialogInfoResponse 
                    { 
                        Id = chat.Id, 
                        Name = chat.Name, 
                        Photo = 1, 
                        LastMessageDateSend = message.DateSend, 
                        LastMessageText = message.Text 
                    });
                }
            }

            return responses;
        }

        public async Task DeleteMessageAsync(Guid chatId, Guid messagesId)
        {
            Message? message = await _messageRepository.GetAsync(messagesId);
            if(message == null)
            {
                throw new ArgumentException(ResponseErrors.MESSAGE_NOT_FOUND);
            }
            if(message.Destination != chatId)
            {
                throw new InvalidOperationException(ResponseErrors.INVALID_DESTINATION);
            }

            await _messageRepository.GetAsync(messagesId);
        }

        public async Task<IEnumerable<RoleResponse>> GetRolesAsync()
        {
            IEnumerable<UserType> userTypes = await _userTypesRepository.GetAll();

            return userTypes.Select(type => new RoleResponse
            {
                Id = type.Id,
                IsDefault = type.IsDefault,
                Name = type.TypeName,
                Permissions = type.Permissions.Split(';'),
                PriorityLevel = type.PriorityLevel
            });
        }

        public async Task<IEnumerable<BaseUserResponse>> SearchUsersAsync(Guid chatId, string nickname)
        {
            IEnumerable<Guid> users = await _userChatsRepository.GetChatUsers(chatId);
            List<BaseUserResponse> results = new List<BaseUserResponse>();
            foreach (Guid user in users)
            {
                User? foundUser = await _userRepository.GetAsync(user);
                if (foundUser.Nickname.Contains(nickname))
                {
                    results.Add(new BaseUserResponse 
                    { 
                        Id = foundUser.Id, 
                        Name = foundUser.Name, 
                        Surname = foundUser.Surname, 
                        Nickname = foundUser.Nickname 
                    });
                }
            }

            return results;
        }

        public async Task EditPhotoAsync(Guid chatId, Guid fileId)
        {
            Models.File? file = await _fileRepository.GetAsync(fileId);
            if(file == null)
            {
                throw new ArgumentException(ResponseErrors.FILE_NOT_FOUND);
            }

            await _chatRepository.UpdateAsync(chatId, fileId);
        }

        public async Task<Guid> UploadFile(byte[] byteFile)
        {
            var client = new HttpClient();
            var response = await client.PostAsync("http://ure.ru", new ByteArrayContent(byteFile));
            string filename = await response.Content.ReadAsStringAsync();

            Models.File file = new Models.File
            {
                Server = "http://ure.ru",
                Path = filename
            };

            await _fileRepository.CreateAsync(file);
            return file.Id;
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
                return new byte[] {4};
            }
        }
    }
}

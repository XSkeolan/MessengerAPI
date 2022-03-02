using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chats;
        private readonly IUserChatRepository _usersInChats;
        private readonly IUserTypeRepository _userTypes;

        public ChatService(IChatRepository chats, IUserChatRepository users, IUserTypeRepository userTypes)
        {
            _chats = chats;
            _usersInChats = users;
            _userTypes = userTypes;
        }

        public async Task CreateChat(Chat chat, Guid[] inviteUsers)
        {
            await _chats.CreateAsync(chat);
            for(int i = 0;i<inviteUsers.Length;i++)
                await _usersInChats.CreateAsync(new UserGroup 
                { GroupId = chat.Id, UserId = inviteUsers[i], UserTypeId = await _userTypes.GetIdByTypeName("user")});
            await _usersInChats.CreateAsync(new UserGroup { GroupId = chat.Id, UserId = chat.Administrator, UserTypeId = await _userTypes.GetIdByTypeName("admin") });
        }
    }
}

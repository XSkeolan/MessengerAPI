using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IChannelService
    {
        Task ChangeCreatorAsync(Guid channelId, Guid userId);
        Task ChangeDescriptionAsync(Guid channelId, string newDescription);
        Task CreateChannelAsync(Channel channel);
        Task CreateInvitationLinkAsync(ChannelLink channelLink);
        Task DeleteChannelAsync(Guid channelId);
        Task DeleteInvitationLinkAsync(Guid channelId);
        Task EditPhotoAsync(Guid channelId, Guid filePhotoId);
        Task<Channel> GetChannelAsync(Guid channelId);
        Task<Channel> JoinAsync(Guid channelId);
        Task<Channel> JoinByLinkAsync(string token);
        Task LeaveAsync(Guid channelId);
        Task RenameChannelAsync(Guid channelId, string newName);

        Task<IEnumerable<Channel>> FindChannelByName(string channelName);
    }
}
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using Dapper;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(IOptions<Connections> options, IServiceContext serviceContext) : base(options, serviceContext) { }

        public override async Task CreateAsync(Chat chat)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("INSERT INTO groups (id, name, description, datecreated, photoid, isdeleted) " +
                    "VALUES(@Id, @Name, @Description, @DateCreated, @PhotoId, @IsDeleted)", chat);
            });
        }

        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE groups SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public override async Task<Chat?> GetAsync(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Chat?>("SELECT groups.id, name, description, datecreated, photoid, groups.isdeleted FROM (" +
                    "SELECT * FROM groups" +
                    "WHERE id=@Id AND isdeleted=false" +
                    ") AS groups" +
                    "JOIN usergroup ON groups.id=groupid AND userid=@UserId AND usergroup.isdeleted=false", new { Id = id, _serviceContext.UserId });
            });
        }

        public async Task UpdateAsync(Guid id, string name, string description)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE groups SET name=@Name, description=@Description WHERE id=@Id AND isdeleted=false", 
                    new { Id = id, Name = name, Description = description });
            });
        }

        public async Task UpdateAsync(Guid id, Guid photoId)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE groups SET photoId=@PhotoId WHERE id=@Id AND isdeleted=false", new { Id = id, PhotoId = photoId });
            });
        }

        public async Task<Message?> GetLastMessage(Guid id)
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Message?>("SELECT id, text, datesend, \"from\" FROM messages " +
                    "WHERE destination=@Destination AND datesend = (SELECT max(datesend) FROM messages WHERE destination=@Destination)", new { Destination = id});
            });
        }
    }
}

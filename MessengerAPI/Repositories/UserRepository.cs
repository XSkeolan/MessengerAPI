using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IOptions<Connections> options) : base(options) { }

        public override async Task CreateAsync(User user)
        {
            user.Id = await Execute(async (conn) =>
            {
                return await conn.QueryFirstOrDefaultAsync<Guid>("INSERT INTO Users (nickname, password, phonenumber, name, surname) " +
                "VALUES(@Nickname, @Password, @Phonenumber, @Name, @Surname) RETURNING id", user);
            });
        }
        
        public override async Task DeleteAsync(Guid id)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Users SET isdeleted=@IsDeleted WHERE id=@Id AND isdeleted=false", new { IsDeleted = true, Id = id });
            });
        }

        public async Task<User?> FindByNicknameAsync(string nickname)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE nickname=@Nickname AND isdeleted=false", new { nickname });
                return users.FirstOrDefault();
            });   
        }

        public async Task<User?> FindByPhonenumberAsync(string phonenumber)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE phonenumber=@Phonenumber AND isdeleted=false", new { phonenumber });
                return users.FirstOrDefault();
            });
        }

        public async Task<User?> FindByConfirmedEmailAsync(string email)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE email=@Email AND isconfirmed=@IsConfirmed AND isdeleted=false", new { Email=email, IsConfirmed=true });
                return users.FirstOrDefault();
            });
        }

        public override async Task<User?> GetAsync(Guid id)
        {
            return await Execute(async (conn) => 
            { 
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE id=@Id", new { id }); 
                return users.FirstOrDefault(); 
            });
        }

        public async Task UpdateAsync(Guid id, string email, bool isConfimed = false)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Users SET email=@Email, isconfirmed=@IsConfirmed WHERE id=@Id AND isdeleted=false", new { Email = email, isConfimed = isConfimed});
            });
        }

        public async Task UpdateAsync(Guid id, string name, string surname)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Users SET name=@Name, surname=@Surname WHERE id=@Id AND isdeleted=false", new { Name = name, Surname = surname });
            });
        }

        public async Task UpdateAsync(Guid id, string nickname)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Users SET nickname=@Nickname WHERE id=@Id AND isdeleted=false", new { Nickname=nickname });
            });
        }

        public async Task ChangePassword(Guid id, string hashedPassword)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Users SET password=@Password WHERE id=@Id AND isdeleted=false", new { Password = hashedPassword });
            });
        }
    }
}
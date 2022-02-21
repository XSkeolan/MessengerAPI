using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
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
                return await conn.ExecuteAsync("UPDATE Users SET isdeleted=@IsDeleted FROM Users WHERE id=@Id", new { IsDeleted = true, Id = id });
            });
        }

        public async Task<User?> FindByNicknameAsync(string nickname)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE nickname=@Nickname", new { nickname });
                return users.FirstOrDefault();
            });   
        }

        public async Task<User?> FindByPhonenumberAsync(string phonenumber)
        {
            return await Execute(async (conn) =>
            {
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE phonenumber=@Phonenumber", new { phonenumber });
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

        public override async Task UpdateAsync(User user)
        {
            await Execute(async (conn) =>
            {
                return await conn.ExecuteAsync("UPDATE Users SET nickname=@Nickname, password=@Password, phonenumber=@Phonenumber, name=@Name, " +
                    "surname=@Surname, email=@Email, isconfirmed=@IsConfirmed WHERE id=@Id", user);
            });
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Execute(async (conn) =>
            {
                return await conn.QueryAsync<User>("SELECT * FROM Users");
            });
        }
    }
}
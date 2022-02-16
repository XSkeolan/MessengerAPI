using Dapper;
using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Npgsql;
using System.Data;

namespace MessengerAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string conn)
        {
            _connectionString = conn;
        }

        public async Task CreateAsync(User user)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                IEnumerable<Guid> id = await conn.QueryAsync<Guid>("INSERT INTO Users (nickname, password, phonenumber, name, surname) " +
                    "VALUES(@Nickname, @Password, @Phonenumber, @Name, @Surname) RETURNING id", user);
                user.Id = id.FirstOrDefault();
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                await conn.ExecuteAsync("UPADETE Users SET isdeleted=@IsDeleted FROM Users WHERE id=@Id", new { IsDeleted = true, Id = id });
            }
        }

        public async Task<User?> FindByNicknameAsync(string nickname)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE nickname=@Nickname", new { nickname });
                return users.FirstOrDefault();
            }
        }

        public async Task<User?> FindByPhonenumberAsync(string phonenumber)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE phonenumber=@Phonenumber", new { phonenumber });
                return users.FirstOrDefault();
            }
        }

        public async Task<User?> GetAsync(Guid id)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                IEnumerable<User> users = await conn.QueryAsync<User>("SELECT * FROM Users WHERE id=@Id", new { id });
                return users.FirstOrDefault();
            }
        }
        public async Task UpdateAsync(User user)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                await conn.ExecuteAsync("UPDATE Users SET nickname=@Nickname, password=@Password, phonenumber=@Phonenumber, name=@Name, " +
                    "surname=@Surname, email=@Email, isconfirmed=@IsConfirmed WHERE id=@Id", user);
            }
        }
    }
}
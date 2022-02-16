using Dapper;
using MessengerAPI.Models;
using Npgsql;
using System.Data;

namespace MessengerAPI.Repos
{
    public class UserRepository: IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string conn)
        {
            _connectionString = conn;
        }

        public void Create(User user)
        {
            using (IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                Guid? id = conn.Query<Guid>("INSERT INTO Users (nickname, password, phonenumber, name, surname) " +
                    "VALUES(@Nickname, @Password, @Phonenumber, @Name, @Surname) RETURNING id", user).FirstOrDefault();
                user.Id = id.Value;
            }
        }
        public void Delete(Guid id)
        {
            using(IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute("UPADETE Users SET isdeleted=@IsDeleted FROM Users WHERE id=@Id", new { IsDeleted=true, Id=id});
            }
        }

        public User? FindByPhonenumber(string phonenumber)
        {
            using(IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                return conn.Query<User>("SELECT * FROM Users WHERE phonenumber=@Phonenumber", new { phonenumber }).FirstOrDefault();
            }
        }

        public User? Get(Guid id)
        {
            using(IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                return conn.Query<User>("SELECT * FROM Users WHERE id=@Id", new { id }).FirstOrDefault();
            }
        }
        public void Update(User user)
        {
            using(IDbConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute("UPDATE Users SET nickname=@Nickname, password=@Password, phonenumber=@Phonenumber, name=@Name, " +
                    "surname=@Surname, email=@Email, isconfirmed=@IsConfirmed WHERE id=@Id", user);
            }
        }
    }
}
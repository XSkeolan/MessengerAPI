﻿using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByPhonenumberAsync(string phonenumber);
        Task<IEnumerable<User>> FindByNicknameAsync(string nickname);
        Task<User?> FindByConfirmedEmailAsync(string email);
        Task DeleteAsync(Guid id, string reason);
    }
}

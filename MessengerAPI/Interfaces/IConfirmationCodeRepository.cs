﻿using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IConfirmationCodeRepository : IRepository<ConfirmationCode>
    {
        public Task<bool> UnUsedCodeExists(string code);
        Task<ConfirmationCode?> GetUnsedCodeByUser(Guid userId);
        Task<IEnumerable<ConfirmationCode>> GetUnusedValidCode();
    }
}
using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IConfirmationCodeRepository : IRepository<ConfirmationCode>
    {
        public Task<bool> UnUsedCodeExists(string code);
        public Task UpdateAsync(Guid id, string code);
        Task<ConfirmationCode> GetUnsedCodeByUser(Guid userId);
        Task UpdateAsync(Guid id, bool isused);
    }
}
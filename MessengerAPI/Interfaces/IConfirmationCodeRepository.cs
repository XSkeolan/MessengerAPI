using MessengerAPI.Interfaces;
using MessengerAPI.Models;

namespace MessengerAPI.Repositories
{
    public interface IConfirmationCodeRepository : IRepository<ConfirmationCode>
    {
        public Task<bool> UnUsedCodeExists(string code);
        public Task<bool> UserHasUnUsedCode(Guid userId);
        public Task UpdateAsync(Guid id, string code);
    }
}
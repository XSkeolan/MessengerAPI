using MessengerAPI.Models;
using MessengerAPI.Interfaces;
using MessengerAPI.Options;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Repositories
{
    public class ConfirmationCodeRepository : BaseRepository<ConfirmationCode>, IConfirmationCodeRepository
    {
        private readonly TimeSpan _validTime;
        public ConfirmationCodeRepository(IOptions<Connections> options, IOptions<CodeOptions> codeOptions) : base(options)
        {
            _validTime = TimeSpan.FromSeconds(codeOptions.Value.ValidCodeTime);
        }

        public async Task<bool> UnUsedCodeExists(string codeHash)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.AndOperation(
                cond.EqualOperation("code", codeHash, EqualOperations.Equal),
                cond.EqualOperation("isused", false, EqualOperations.Equal)),
                cond.EqualOperation("isdeleted", false, EqualOperations.Equal));
            return (await GetByConditions(cond)).Any();
        }

        public async Task<ConfirmationCode?> GetUnsedCodeByUser(Guid userId)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.AndOperation(
                cond.EqualOperation("userid", userId, EqualOperations.Equal),
                cond.EqualOperation("isused", false, EqualOperations.Equal)),
                cond.EqualOperation("isdeleted", false, EqualOperations.Equal));
            return (await GetByConditions(cond)).FirstOrDefault();
        }

        public async Task<IEnumerable<ConfirmationCode>> GetUnusedValidCode()
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.AndOperation(
                cond.EqualOperation("isused", false, EqualOperations.Equal),
                cond.EqualOperation("datestart", "timestamp without time zone", DateTime.UtcNow.Add(-_validTime), "timestamp without time zone", EqualOperations.MoreEqual)),
                cond.EqualOperation("isdeleted", false, EqualOperations.Equal));
            return await GetByConditions(cond);
        }
    }
}

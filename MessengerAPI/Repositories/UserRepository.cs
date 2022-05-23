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

        public async Task DeleteAsync(Guid id, string reason)
        {
            await UpdateAsync(id, "reason", reason);
            await DeleteAsync(id);
        }

        public async Task<IEnumerable<User>> FindByNicknameAsync(string nickname)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.LikeOperation("nickname", $"%{nickname}%");
            return await GetByConditions(cond);
        }

        public async Task<User?> FindByPhonenumberAsync(string phonenumber)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.EqualOperation("phonenumber", phonenumber, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond)).FirstOrDefault();
        }

        public async Task<User?> FindByConfirmedEmailAsync(string email)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.AndOperation(cond.EqualOperation("email", email, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal)),cond.EqualOperation("isconfirmed", true, EqualOperations.Equal));

            return (await GetByConditions(cond)).FirstOrDefault();
        }
    }
}
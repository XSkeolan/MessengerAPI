using MessengerAPI.Interfaces;
using MessengerAPI.Models;
using Microsoft.Extensions.Options;
using MessengerAPI.Options;

namespace MessengerAPI.Repositories
{
    public class UserTypeRepository : BaseRepository<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(IOptions<Connections> options) : base(options) { }

        public async Task<UserType?> GetByTypeNameAsync(string typeName)
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.EqualOperation("typename", typeName, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond)).FirstOrDefault();
        }

        public async Task<UserType> GetDefaultType()
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.AndOperation(cond.EqualOperation("isdefault", true, EqualOperations.Equal), cond.EqualOperation("isdeleted", false, EqualOperations.Equal));

            return (await GetByConditions(cond)).First();
        }

        public async Task<IEnumerable<UserType>> GetAll()
        {
            ConditionBuilder cond = Builder.Condition;
            cond = cond.EqualOperation("isdeleted", false, EqualOperations.Equal);

            return await GetByConditions(cond);
        }
    }
}

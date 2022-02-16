using MessengerAPI.Models;

namespace MessengerAPI.Repos
{
    public interface ISessionRepository
    {
        void Create(Session session);
        void End(Guid id);
        Session? Get(Guid id);
    }
}
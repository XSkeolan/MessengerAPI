using MessengerAPI.Models;

namespace MessengerAPI.Interfaces
{
    public interface IServiceContext
    {
        public Guid UserId { get; }
        public Guid SessionId { get; }

        public void ConfigureSession(Session session);
    }
}

using MessengerAPI.DTOs;

namespace MessengerAPI.Services
{
    public interface ISignUpService
    {
        SignUpResponse SignUp(SignUpRequest user, string device);
    }
}

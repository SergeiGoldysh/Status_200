using Status_200.Models;

namespace Status_200WebAPI.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(string email, string password);
    }
}

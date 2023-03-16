using Microsoft.EntityFrameworkCore;
using Status_200;
using Status_200.Models;
using Status_200WebAPI.Interfaces;

namespace Status_200WebAPI.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public Task<User> Authenticate(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Email == email && x.Password == password);
            return Task.FromResult(user);
        }
    }
}

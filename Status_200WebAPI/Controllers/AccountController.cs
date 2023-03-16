using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Status_200;
using Status_200.Models;
using Status_200WebAPI.Models;

namespace Status_200WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("UserRegistr", Name = "UserRegistr")]
        public async Task<IActionResult> UserRegistr([FromBody] UserModel userModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!userModel.Email.Contains("@"))
            {
                return BadRequest("Email is not correct");
            }
            if(_context.Users.Count(x=>x.Email.Trim() == userModel.Email.Trim()) > 0)
            {
                return BadRequest("Error, Email already exists");  //Ошибка, электронная почта уже существует
            }
            try
            {
                var user = new User();
                user.FirstName = userModel.FirstName;
                user.SecondName = userModel.SecondName;
                user.Email = userModel.Email;
                user.Password = userModel.Password;
                user.StatusId = 1;
                user.RoleId = 2;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok("you have successfully registered"); //вы успешно зарегистрировались
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Authorization", Name = "Authorization")]
        public async Task<IActionResult> Authorization(string email, string password)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(x=>x.Email == email && x.Password == password);

            if (user != null)
            {
                if (user.StatusId == 1)
                {
                    var usersDto = new UserViewDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        SecondName = user.SecondName,
                        StatusName = user.Status.NameStatus,
                        RoleName = user.Role.RoleName,
                        Email = user.Email,
                        Password = user.Password,
                        StatusId = user.StatusId,
                        RoleId = user.RoleId
                    };
                    return Ok(usersDto);
                }
                else
                    return BadRequest("User " + user.Email + " is in the status " + user.Status.NameStatus); //Пользователь+ находится в статусе+
            }
            else
            {
                return BadRequest("Login or password entered incorrectly");  //Не верно введен логин или пароль
            }
        }



    }
}

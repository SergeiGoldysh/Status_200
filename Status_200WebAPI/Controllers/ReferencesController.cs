using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Status_200;
using Status_200WebAPI.Models;

namespace Status_200WebAPI.Controllers
{
    [Authorize(Roles = "1,2")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReferencesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
       
        [Route("GetRoles",Name = "GetRoles")]
        public async Task<IActionResult> GetRoles() => Ok(await _context.Roles.ToListAsync());

        [HttpGet]
        [Route("StatusesTask", Name = "StatusesTask")]
        public async Task<IActionResult> StatusesTask() => Ok(await _context.StatusesTask.ToListAsync());

        [HttpGet]
        [Route("UsersStatus", Name = "UsersStatus")]
        public async Task<IActionResult> UsersStatus() => Ok(await _context.UsersStatus.ToListAsync());
        [HttpGet]
        [Route("GetUsers", Name = "GetUsers")]
        //[Authorize(Roles = "1")]
        public async Task<IActionResult> GetUsers()
        {
            //var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var users = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Status)
                .ToListAsync();

            var usersDto = users.Select(u => new UserViewDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                SecondName = u.SecondName,
                StatusName = u.Status.NameStatus,
                RoleName = u.Role.RoleName,
                Email = u.Email,
                StatusId = u.StatusId,
                RoleId = u.RoleId,
                Password = "*******"
            });

            return Ok(usersDto.ToList());
        }


    }
}

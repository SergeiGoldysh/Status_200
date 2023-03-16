using Status_200.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Status_200WebAPI.Models
{
    public class UserViewDto
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string StatusName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int StatusId { get; set; }
        public int RoleId { get; set; }
    }
}
    
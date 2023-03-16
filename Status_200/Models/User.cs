using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status_200.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        [ForeignKey("Status")]
        public byte StatusId { get; set; } 
        public UserStatus Status { get; set; }
        [ForeignKey("Role")]
        public byte RoleId { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

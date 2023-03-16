using System.ComponentModel.DataAnnotations;

namespace Status_200WebAPI.Models
{
    public class ReportAddDTO
    {
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string AssignedTasks { get; set; }
        public string WorkReport { get; set; } //Description
        [Required]
        public int UserId { get; set; }
    }
}

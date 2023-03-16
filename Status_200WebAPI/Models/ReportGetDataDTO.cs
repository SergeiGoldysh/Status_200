using System.ComponentModel.DataAnnotations;

namespace Status_200WebAPI.Models
{
    public class ReportGetDataDTO
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string AssignedTasks { get; set; }
        public string WorkReport { get; set; }
        public string StatusName { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public byte StatusTaskId { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateClosed { get; set; }
    }
}

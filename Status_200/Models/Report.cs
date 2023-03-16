using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Status_200.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string AssignedTasks { get; set; }
        [ForeignKey("Status")]
        public byte StatusReports { get; set; }
        public StatusTask Status { get; set; }

        [ForeignKey("UsersId")]
        public int UserId { get; set; }
        public User UsersId { get; set; }
        public string WorkReport { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateClosed { get; set; }
    }
}

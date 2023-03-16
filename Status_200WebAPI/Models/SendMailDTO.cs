using System.ComponentModel.DataAnnotations;

namespace Status_200WebAPI.Models
{
    public class SendMailDTO
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string subject { get; set; }
        [Required]
        public string message { get; set; }
        public IFormFile formFile { get; set; }
    }
}

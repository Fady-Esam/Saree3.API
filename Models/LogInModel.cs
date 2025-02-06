using System.ComponentModel.DataAnnotations;

namespace Saree3.API.Models
{
    public class LogInModel
    {
        [Required(ErrorMessage = "Email Field is Required")]
        [EmailAddress]
        [MaxLength(40, ErrorMessage = "Your Email must not exceed 40 characters")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Field is Required")]
        [MaxLength(50, ErrorMessage = "Your Password must not exceed 50 characters")]
        public string Password { get; set; }
    }
}

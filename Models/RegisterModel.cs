using System.ComponentModel.DataAnnotations;

namespace Saree3.API.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "UserName Field is Required")]
        [MaxLength(30, ErrorMessage = "Your UserName must not exceed 30 characters")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email Field is Required")]
        [EmailAddress]
        [MaxLength(40, ErrorMessage = "Your Email must not exceed 40 characters")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Field is Required")]
        [MaxLength(50, ErrorMessage = "Your Password must not exceed 50 characters")]
        public string Password { get; set; }
    }
}

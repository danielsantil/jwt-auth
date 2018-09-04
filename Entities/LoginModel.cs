using System.ComponentModel.DataAnnotations;

namespace TestAuth.Entities
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
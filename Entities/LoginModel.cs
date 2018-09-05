using System.ComponentModel.DataAnnotations;

namespace TestAuth.Entities
{
    public class LoginModel
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
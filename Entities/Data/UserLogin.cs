using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Entities.Data
{
    public class UserLogin
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }
        [Required]
        public byte[] Hash { get; set; }
        [Required]
        public byte[] Salt { get; set; }
    }
}

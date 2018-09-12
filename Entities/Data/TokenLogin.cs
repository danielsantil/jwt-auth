using System;
using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Entities.Data
{
    public class TokenLogin
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        public string Origin { get; set; }
        [Required]
        public DateTime GeneratedOn { get; set; }
        [Required]
        public DateTime Expiration { get; set; }
    }

}
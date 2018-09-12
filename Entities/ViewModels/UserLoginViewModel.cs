using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuth.Entities.ViewModels
{
    public class UserLoginViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }
        [Required, MaxLength(100)]
        public string Password { get; set; }

        public List<Claim> GetClaims()
        {
            var customClains = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Email, Email)
            };
            return customClains;
        }
    }
}
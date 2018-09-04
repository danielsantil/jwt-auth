using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TestAuth.Services;
using System.IdentityModel.Tokens.Jwt;
using TestAuth.Entities;
using System.Collections.Generic;
using System.Web.Http;

namespace TestAuth.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private IJwtAuthentication _authService;

        public AuthController(IJwtAuthentication authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (!IsLoginValid(login)) return Unauthorized();

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Email, login.Email)
            };

            return Ok(_authService.GetToken(claims));
        }

        private bool IsLoginValid(LoginModel login)
        {
            // TODO this is a placeholder. it needs to implement actual login validation
            return ModelState.IsValid;
        }
    }
}
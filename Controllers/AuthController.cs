using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TestAuth.Services;
using System.IdentityModel.Tokens.Jwt;
using TestAuth.Entities;
using System.Collections.Generic;
using System.Web.Http;
using TestAuth.Services.Data;

namespace TestAuth.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private IJwtAuthentication _authService;
        private ILoginData _loginData;

        public AuthController(IJwtAuthentication authService, ILoginData loginData)
        {
            _authService = authService;
            _loginData = loginData;
        }

        [HttpPost]
        public IActionResult Token(LoginModel model)
        {
            if (!_loginData.IsLoginValid(model)) return Unauthorized();

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Email, model.Email)
            };

            return Ok(_authService.GetToken(claims));
        }
    }
}
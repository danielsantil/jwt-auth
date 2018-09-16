using System.Threading.Tasks;
using JwtAuth.Filters;
using JwtAuth.Services;
using JwtAuthModels.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers
{
    [ModelValidationFilter]
    public class AuthController : Controller
    {
        private readonly IUserAuthentication _userService;

        public AuthController(IUserAuthentication userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            return Ok(await _userService.Register(model));
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            return Ok(await _userService.Authenticate(model));
        }

        [HttpPost]
        public async Task<IActionResult> NewRefreshToken(TokenViewModel model)
        {
            return Ok(await _userService.GetNewRefreshToken(model));
        }

        [HttpPost]
        public async Task<IActionResult> InvalidateOthers(TokenViewModel model)
        {
            return Ok(await _userService.InvalidateRefreshTokens(model));
        }
    }
}
using JwtAuth.Services;
using JwtAuthModels.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers
{
    public class AuthController : Controller
    {
        private const string InvalidRefreshToken = "Invalid refresh token";
        private const string RefreshTokenExpired = "Refresh token not longer valid.";
        private readonly IUserAuthentication _userService;

        public AuthController(IUserAuthentication userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register(UserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(_userService.Register(model));
        }

        [HttpPost]
        public IActionResult Login(UserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(_userService.Authenticate(model));
        }

        //[HttpPost]
        //public IActionResult RefreshToken(string token, string refreshToken)
        //{
        //    try
        //    {
        //        if (token == null) return BadRequest();

        //        UserLoginViewModel user = GetUserFromToken(token);
        //        TokenLogin oldTokenEntity = ValidateRefreshToken(user.Id, refreshToken);

        //        string newAccessToken = _authService.GetToken(user.GetClaims());
        //        string newRefreshToken = _authService.GetRefreshToken();

        //        _loginData.DeleteRefreshToken(oldTokenEntity);
        //        TokenLogin newTokenEntity = BuildTokenEntity(user.Id, newRefreshToken);
        //        _loginData.SaveRefreshToken(newTokenEntity);

        //        var result = new
        //        {
        //            accessToken = newAccessToken,
        //            refreshToken = newRefreshToken
        //        };
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //public IActionResult InvalidateOthers(string token, string refreshToken)
        //{
        //    try
        //    {
        //        if (token == null) return BadRequest();

        //        UserLoginViewModel user = this.GetUserFromToken(token);
        //        ValidateRefreshToken(user.Id, refreshToken);
        //        int count = _loginData.DeleteDistinctRefreshTokens(user.Id, refreshToken);

        //        var result = new
        //        {
        //            tokensInvalidated = count
        //        };
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //private UserLoginViewModel GetUserFromToken(string token)
        //{
        //    var principal = _authService.GetPrincipalsFromExpired(token);
        //    string email = principal.Identities.FirstOrDefault().Claims.FirstOrDefault().Value;
        //    int userId = _loginData.GetUserId(email);

        //    return new UserLoginViewModel
        //    {
        //        Id = userId,
        //        Email = email
        //    };
        //}

        //private TokenLogin ValidateRefreshToken(int userId, string refreshToken)
        //{
        //    TokenLogin oldTokenEntity = _loginData.GetRefreshTokenEntity(userId, refreshToken);
        //    if (oldTokenEntity == null || DateTime.Now.CompareTo(oldTokenEntity.Expiration) >= 0)
        //    {
        //        throw new Exception(RefreshTokenExpired);
        //    }
        //    return oldTokenEntity;
        //}
    }
}
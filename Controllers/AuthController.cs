using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TestAuth.Services;
using System.IdentityModel.Tokens.Jwt;
using TestAuth.Entities;
using System.Collections.Generic;
using System.Web.Http;
using TestAuth.Services.Data;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TestAuth.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private const string InvalidRefreshToken = "Invalid refresh token";
        private IHttpContextAccessor _http;
        private IConfiguration _configuration;
        private IJwtAuthentication _authService;
        private ILoginData _loginData;

        public AuthController(IHttpContextAccessor httpContext, IConfiguration configuration, IJwtAuthentication authService, ILoginData loginData)
        {
            _http = httpContext;
            _configuration = configuration;
            _authService = authService;
            _loginData = loginData;
        }

        [HttpPost]
        public IActionResult Token(UserLogin model)
        {
            if (!_loginData.IsLoginValid(model)) return Unauthorized();

            string token = _authService.GetToken(model.GetClaims());
            string refreshToken = _authService.GetRefreshToken();
            TokenLogin tokenModel = BuildTokenEntity(model.Id, refreshToken);
            _loginData.SaveRefreshToken(tokenModel);

            var result = new
            {
                accessToken = token,
                refreshToken = refreshToken
            };
            return Ok(result);
        }

        [HttpPost]
        public IActionResult RefreshToken(string token, string refreshToken)
        {
            if (token == null) return BadRequest();

            var principal = _authService.GetPrincipalsFromExpired(token);
            var email = principal.Identities.FirstOrDefault().Claims.FirstOrDefault().Value;
            TokenLogin oldTokenEntity;
            int userId = _loginData.GetUserId(email);
            if (!_loginData.IsRefreshTokenValid(userId, refreshToken, out oldTokenEntity))
            {
                return BadRequest(InvalidRefreshToken);
            }
            var userModel = new UserLogin { Email = email };
            string newAccessToken = _authService.GetToken(userModel.GetClaims());
            string newRefreshToken = _authService.GetRefreshToken();
            _loginData.DeleteRefreshToken(oldTokenEntity);
            TokenLogin newTokenEntity = BuildTokenEntity(userId, newRefreshToken);
            _loginData.SaveRefreshToken(newTokenEntity);

            var result = new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            };
            return Ok(result);
        }

        private TokenLogin BuildTokenEntity(int userId, string refreshToken)
        {
            TokenLogin newTokenEntity = new TokenLogin
            {
                UserId = userId,
                RefreshToken = refreshToken,
                GeneratedOn = DateTime.Now,
                Expiration = DateTime.Now.AddSeconds(_configuration.GetValue<double>("JWT:RefreshExpireSeconds")),
                Origin = _http.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString()
            };

            return newTokenEntity;
        }
    }
}
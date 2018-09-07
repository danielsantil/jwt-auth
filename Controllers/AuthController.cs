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
        private const string RefreshTokenExpired = "Refresh token not longer valid.";
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
            int refreshCount = _loginData.CountRefreshTokens(model.Id);

            var result = new
            {
                accessToken = token,
                refreshToken = refreshToken,
                refreshTokensCount = refreshCount
            };
            return Ok(result);
        }

        [HttpPost]
        public IActionResult RefreshToken(string token, string refreshToken)
        {
            try
            {
                if (token == null) return BadRequest();

                UserLogin user = this.GetUserFromToken(token);
                TokenLogin oldTokenEntity = ValidateRefreshToken(user.Id, refreshToken);

                string newAccessToken = _authService.GetToken(user.GetClaims());
                string newRefreshToken = _authService.GetRefreshToken();

                _loginData.DeleteRefreshToken(oldTokenEntity);
                TokenLogin newTokenEntity = BuildTokenEntity(user.Id, newRefreshToken);
                _loginData.SaveRefreshToken(newTokenEntity);

                var result = new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult InvalidateOthers(string token, string refreshToken)
        {
            try
            {
                if (token == null) return BadRequest();

                UserLogin user = this.GetUserFromToken(token);
                ValidateRefreshToken(user.Id, refreshToken);
                int count = _loginData.DeleteDistinctRefreshTokens(user.Id, refreshToken);

                var result = new
                {
                    tokensInvalidated = count
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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

        private UserLogin GetUserFromToken(string token)
        {
            var principal = _authService.GetPrincipalsFromExpired(token);
            string email = principal.Identities.FirstOrDefault().Claims.FirstOrDefault().Value;
            int userId = _loginData.GetUserId(email);

            return new UserLogin
            {
                Id = userId,
                Email = email
            };
        }

        private TokenLogin ValidateRefreshToken(int userId, string refreshToken)
        {
            TokenLogin oldTokenEntity = _loginData.GetRefreshTokenEntity(userId, refreshToken);
            if (oldTokenEntity == null || DateTime.Now.CompareTo(oldTokenEntity.Expiration) >= 0)
            {
                throw new Exception(RefreshTokenExpired);
            }
            return oldTokenEntity;
        }
    }
}
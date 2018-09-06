using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TestAuth.Services
{
    public class JwtAuthentication : IJwtAuthentication
    {
        private const string InvalidToken = "Invalid token";
        private IConfiguration _conf;

        public JwtAuthentication(IConfiguration configuration)
        {
            _conf = configuration;
        }

        public string GetToken(List<Claim> customClaims)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["JWT:SecretKey"]));
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _conf["JWT:Issuer"],
                audience: _conf["JWT:Issuer"],
                claims: customClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(_conf.GetValue<double>("JWT:ExpireSeconds")),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public string GetRefreshToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomNum = new byte[32];
                rng.GetBytes(randomNum);
                return Convert.ToBase64String(randomNum);
            }
        }

        public ClaimsPrincipal GetPrincipalsFromExpired(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _conf["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = _conf["JWT:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["JWT:SecretKey"])),
                ValidateLifetime = false
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(InvalidToken);
            }

            return principal;
        }
    }
}
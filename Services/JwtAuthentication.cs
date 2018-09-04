using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TestAuth.Services
{
    public class JwtAuthentication : IJwtAuthentication
    {
        private IConfiguration _conf;

        public JwtAuthentication(IConfiguration configuration)
        {
            _conf = configuration;
        }

        public string GetToken(List<Claim> claims)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["JWT:SecretKey"]));
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _conf["JWT:Issuer"],
                audience: _conf["JWT:Issuer"],
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public string GetRefreshToken()
        {
            throw new System.NotImplementedException();
        }
    }
}
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
    }
}
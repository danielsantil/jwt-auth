using System.Collections.Generic;
using System.Security.Claims;
using JwtAuthModels.Data;

namespace JwtAuth.Services
{
    public interface IJwtAuthentication
    {
        string GetToken(List<Claim> customClaims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalsFromExpired(string token);
        Token BuildTokenEntity(int userId, string refreshToken);
        List<Claim> GetClaims(string email);
    }
}
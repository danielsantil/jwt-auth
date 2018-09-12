using System.Collections.Generic;
using System.Security.Claims;

namespace JwtAuth.Services
{
    public interface IJwtAuthentication
    {
        string GetToken(List<Claim> customClaims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalsFromExpired(string token);
    }
}
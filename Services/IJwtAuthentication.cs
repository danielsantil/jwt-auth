using System.Collections.Generic;
using System.Security.Claims;

namespace TestAuth.Services
{
    public interface IJwtAuthentication
    {
        string GetToken(List<Claim> customClaims);
    }
}
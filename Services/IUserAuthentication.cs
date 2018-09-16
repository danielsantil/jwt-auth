using System.Threading.Tasks;
using JwtAuthModels.ViewModels;

namespace JwtAuth.Services
{
    public interface IUserAuthentication
    {
        Task<ApiResponse> Register(UserViewModel user);
        Task<ApiResponse> Authenticate(UserViewModel user);
        Task<ApiResponse> GetNewRefreshToken(TokenViewModel modelToken);
        Task<ApiResponse> InvalidateRefreshTokens(TokenViewModel model);
    }
}

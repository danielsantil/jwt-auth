using JwtAuthModels.ViewModels;

namespace JwtAuth.Services
{
    public interface IUserAuthentication
    {
        ApiResponse Register(UserViewModel user);
        ApiResponse Authenticate(UserViewModel user);
    }
}

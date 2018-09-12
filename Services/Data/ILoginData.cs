using JwtAuth.Entities.ViewModels;
using JwtAuth.Entities.Data;

namespace JwtAuth.Services.Data
{
    public interface ILoginData
    {
        bool IsLoginValid(UserLoginViewModel model);
        object RegisterUser(UserLoginViewModel credentials);
        void SaveRefreshToken(TokenLogin model);
        TokenLogin GetRefreshTokenEntity(int userId, string refreshToken);
        void DeleteRefreshToken(TokenLogin model);
        int GetUserId(string email);
        int DeleteDistinctRefreshTokens(int userId, string currentRefreshToken);
        int CountRefreshTokens(int userId);
    }
}
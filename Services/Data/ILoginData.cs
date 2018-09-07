using TestAuth.Entities;

namespace TestAuth.Services.Data
{
    public interface ILoginData
    {
        bool IsLoginValid(UserLogin model);
        object RegisterUser(UserLogin credentials);
        void SaveRefreshToken(TokenLogin model);
        TokenLogin GetRefreshTokenEntity(int userId, string refreshToken);
        void DeleteRefreshToken(TokenLogin model);
        int GetUserId(string email);
        int DeleteDistinctRefreshTokens(int userId, string currentRefreshToken);
        int CountRefreshTokens(int userId);
    }
}
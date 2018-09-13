using JwtAuthModels.Data;

namespace JwtAuth.Services.Data
{
    public interface ILoginData
    {
        void RegisterUser(User credentials);
        bool UserExists(User newUser);
        User GetUser(string email);
        void SaveRefreshToken(Token model);
        int GetUserRefreshTokens(int userId);
        //Token GetRefreshTokenEntity(int userId, string refreshToken);
        //void DeleteRefreshToken(TokenLogin model);
        //int DeleteDistinctRefreshTokens(int userId, string currentRefreshToken);
    }
}
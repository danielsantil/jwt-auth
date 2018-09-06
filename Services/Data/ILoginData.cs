using TestAuth.Entities;

namespace TestAuth.Services.Data
{
    public interface ILoginData
    {
        bool IsLoginValid(UserLogin model);
        void SaveRefreshToken(TokenLogin model);
        bool IsRefreshTokenValid(int userId, string refreshToken, out TokenLogin tokenEntity);
        void DeleteRefreshToken(TokenLogin model);
        int GetUserId(string email);
    }
}
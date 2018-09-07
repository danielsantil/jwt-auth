using TestAuth.Data;
using TestAuth.Entities;
using System.Linq;
using System;

namespace TestAuth.Services.Data
{
    public class SqlLoginData : ILoginData
    {
        private LoginDbContext _context;

        public SqlLoginData(LoginDbContext context)
        {
            _context = context;
        }
        public bool IsLoginValid(UserLogin model)
        {
            var logged = _context.UserLogin.FirstOrDefault(e =>
            e.Email == model.Email && e.Password == model.Password);
            if (logged != null)
            {
                model.Id = logged.Id;
                return true;
            }

            return false;
        }

        public void SaveRefreshToken(TokenLogin model)
        {
            _context.TokenLogin.Add(model);
            _context.SaveChanges();
        }

        public TokenLogin GetRefreshTokenEntity(int userId, string refreshToken)
        {
            var query = from token in _context.TokenLogin
                        join user in _context.UserLogin
                        on token.UserId equals user.Id
                        where user.Id == userId
                        && token.RefreshToken == refreshToken
                        select new { value = token };

            return query.SingleOrDefault()?.value;
        }

        public void DeleteRefreshToken(TokenLogin model)
        {
            _context.Remove(model);
            _context.SaveChanges();
        }

        public int GetUserId(string email)
        {
            return _context.UserLogin.SingleOrDefault(e => e.Email == email).Id;
        }

        public int DeleteDistinctRefreshTokens(int userId, string currentRefreshToken)
        {
            _context.TokenLogin.RemoveRange(_context.TokenLogin
                .Where(t => t.UserId == userId && t.RefreshToken != currentRefreshToken));
            return _context.SaveChanges();
        }

        public int CountRefreshTokens(int userId)
        {
            return _context.TokenLogin.Count(t => t.UserId == userId);
        }
    }
}
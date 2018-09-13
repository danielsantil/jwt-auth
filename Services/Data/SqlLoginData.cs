using JwtAuth.DataContext;
using JwtAuthModels.Data;
using System.Linq;

namespace JwtAuth.Services.Data
{
    public class SqlLoginData : ILoginData
    {
        private JwtAuthDbContext _context;

        public SqlLoginData(JwtAuthDbContext context)
        {
            _context = context;
        }

        public void RegisterUser(User model)
        {
            _context.User.Add(model);
            _context.SaveChanges();
        }

        public bool UserExists(User newUser)
        {
            return _context.User.Count(x => x.Email == newUser.Email) > 0;
        }

        public User GetUser(string email)
        {
            return _context.User.FirstOrDefault(x => x.Email == email);
        }

        public void SaveRefreshToken(Token model)
        {
            _context.Token.Add(model);
            _context.SaveChanges();
        }

        public int GetUserRefreshTokens(int userId)
        {
            return _context.Token.Count(t => t.UserId == userId);
        }

        //public TokenLogin GetRefreshTokenEntity(int userId, string refreshToken)
        //{
        //    TokenLogin tl = _context.Token
        //                    .SingleOrDefault(x => x.UserLoginId == userId && x.RefreshToken == refreshToken);
        //    return tl;
        //}

        //public void DeleteRefreshToken(TokenLogin model)
        //{
        //    _context.Remove(model);
        //    _context.SaveChanges();
        //}

        //public int DeleteDistinctRefreshTokens(int userId, string currentRefreshToken)
        //{
        //    _context.Token.RemoveRange(_context.Token
        //        .Where(t => t.UserLoginId == userId && t.RefreshToken != currentRefreshToken));
        //    return _context.SaveChanges();
        //}
    }
}
using JwtAuth.DataContext;
using JwtAuth.Entities.ViewModels;
using JwtAuth.Entities.Data;
using System.Linq;
using System.Security.Cryptography;

namespace JwtAuth.Services.Data
{
    public class SqlLoginData : ILoginData
    {
        private JwtAuthDbContext _context;

        public SqlLoginData(JwtAuthDbContext context)
        {
            _context = context;
        }
        public bool IsLoginValid(UserLoginViewModel model)
        {
            //var logged = _context.UserLogin.FirstOrDefault(e =>
            //e.Email == model.Email && e.Password == model.Password);
            //if (logged != null)
            //{
            //    model.Id = logged.Id;
            //    return true;
            //}

            //return false;
            return true;
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

        public object RegisterUser(UserLoginViewModel creds)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
            }


            return true;
        }

        private byte[] GeneratePassword(string plainText, out byte[] salt)
        {
            int SALT_SIZE = 64;
            int HASH_SIZE = 32;
            int HASH_ITERATIONS = 10000;

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            using (Rfc2898DeriveBytes hashFunction = new Rfc2898DeriveBytes(plainText, SALT_SIZE))
            {
                // Generate salt
                salt = new byte[SALT_SIZE];
                rng.GetBytes(salt);

                // Generate hash
                hashFunction.Salt = salt;
                hashFunction.IterationCount = HASH_ITERATIONS;
                byte[] hash = hashFunction.GetBytes(HASH_SIZE);
                return hash;
            }
        }
    }
}
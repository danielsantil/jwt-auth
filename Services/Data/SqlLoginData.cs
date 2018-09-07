using System;
using System.Linq;
using System.Security.Cryptography;
using TestAuth.Data;
using TestAuth.Entities;

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

        public object RegisterUser(UserLogin creds)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
            }


            return true;
        }

        private string GeneratePassword(string plainText)
        {
            int SALT_SIZE = 64;
            int HASH_SIZE = 32;
            int HASH_ITERATIONS = 10000;

            byte[] salt = GenerateSalt(SALT_SIZE);
            byte[] hash = GenerateHash(plainText, salt, HASH_ITERATIONS, HASH_SIZE);
            byte[] appendedHash = new byte[HASH_SIZE + SALT_SIZE];
            Array.Copy(salt, 0, appendedHash, 0, SALT_SIZE);
            Array.Copy(hash, 0, appendedHash, SALT_SIZE, HASH_SIZE);

            return Convert.ToBase64String(appendedHash);
        }

        private byte[] GenerateSalt(int saltSize)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[saltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        private byte[] GenerateHash(string plainText, byte[] salt, int iterations, int hashSize)
        {
            using (var hashFunction = new Rfc2898DeriveBytes(plainText, salt, iterations))
            {
                byte[] hash = hashFunction.GetBytes(hashSize);
                return hash;
            }
        }
    }
}
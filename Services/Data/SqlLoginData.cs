using TestAuth.Data;
using TestAuth.Entities;
using System.Linq;

namespace TestAuth.Services.Data
{
    public class SqlLoginData : ILoginData
    {
        private LoginDbContext _context;

        public SqlLoginData(LoginDbContext context)
        {
            _context = context;
        }
        public bool IsLoginValid(LoginModel model)
        {
            int count = _context.Login.Count(elem =>
                elem.Email == model.Email && elem.Password == model.Password
            );
            return count > 0;
        }
    }
}
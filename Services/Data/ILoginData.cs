using TestAuth.Entities;

namespace TestAuth.Services.Data
{
    public interface ILoginData
    {
        bool IsLoginValid(LoginModel model);
    }
}
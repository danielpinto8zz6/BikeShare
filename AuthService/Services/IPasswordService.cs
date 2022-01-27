using System.Threading.Tasks;

namespace AuthService.Services
{
    public interface IPasswordService
    {
        bool Matches(string password, string passwordHash);
        
        string Hash(string password);
    }
}
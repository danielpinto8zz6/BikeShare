using BCryptNet = BCrypt.Net.BCrypt;

namespace Common.Services
{
    public class PasswordService : IPasswordService
    {
        public bool Matches(string password, string passwordHash)
        {
                return BCryptNet.EnhancedVerify(password, passwordHash);
        }

        public string Hash(string password)
        {
            return BCryptNet.EnhancedHashPassword(password);
        }
    }
}
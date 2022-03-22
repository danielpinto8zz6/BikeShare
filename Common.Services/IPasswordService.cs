namespace Common.Services
{
    public interface IPasswordService
    {
        bool Matches(string password, string passwordHash);
        
        string Hash(string password);
    }
}
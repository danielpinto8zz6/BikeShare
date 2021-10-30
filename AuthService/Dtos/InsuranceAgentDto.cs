using System.Collections.Generic;

namespace AuthService.Dtos
{
    public class InsuranceAgentDto
    {
        public string Login { get; }
        public string Password { get; }
        public string Avatar { get; }
        public List<string> AvailableProducts { get; }

        public InsuranceAgentDto(string login, string password, string avatar, List<string> availableProducts)
        {
            Login = login;
            Password = password;
            Avatar = avatar;
            AvailableProducts = availableProducts;
        }

        public bool PasswordMatches(string passwordToTest) => Password == passwordToTest;
    }
}
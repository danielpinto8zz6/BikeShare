using System.Threading.Tasks;
using AuthService.Gateways;
using Common.Models.Dtos;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserGateway _userGateway;

        public UserService(IUserGateway userGateway)
        {
            _userGateway = userGateway;
        }

        public Task<ApplicationUserDto> GetByUsernameAsync(string username)
        {
            return _userGateway.GetByUsernameAsync(username);
        }
    }
}
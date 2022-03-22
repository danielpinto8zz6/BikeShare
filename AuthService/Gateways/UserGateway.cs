using System.Threading.Tasks;
using AuthService.Gateways.Clients;
using Common.Models.Dtos;

namespace AuthService.Gateways
{
    public class UserGateway : IUserGateway
    {
        private readonly IUserClient _userClient;
        
        public UserGateway(IUserClient userClient)
        {
            _userClient = userClient;
        }

        public Task<ApplicationUserDto> GetByUsernameAsync(string username)
        {
            return _userClient.GetByUsernameAsync(username);
        }
    }
}
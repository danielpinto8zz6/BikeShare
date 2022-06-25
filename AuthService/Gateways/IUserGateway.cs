using System.Threading.Tasks;
using Common.Models.Dtos;

namespace AuthService.Gateways
{
    public interface IUserGateway
    {
        Task<UserDto> GetByUsernameAsync(string username);
    }
}
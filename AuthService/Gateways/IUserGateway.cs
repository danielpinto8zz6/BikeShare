using System.Threading.Tasks;
using Common.Models.Dtos;

namespace AuthService.Gateways
{
    public interface IUserGateway
    {
        Task<ApplicationUserDto> GetByUsernameAsync(string username);
        
        Task<ApplicationUserDto> CreateAsync(ApplicationUserDto applicationUserDto);
    }
}
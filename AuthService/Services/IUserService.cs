using System.Threading.Tasks;
using Common.Models.Dtos;

namespace AuthService.Services
{
    public interface IUserService
    {
        Task<ApplicationUserDto> GetByUsernameAsync(string username);
        
        Task<ApplicationUserDto> CreateAsync(ApplicationUserDto applicationUserDto);
    }
}
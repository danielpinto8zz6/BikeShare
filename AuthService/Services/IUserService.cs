using System.Threading.Tasks;
using Common.Models.Dtos;

namespace AuthService.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByUsernameAsync(string username);
    }
}
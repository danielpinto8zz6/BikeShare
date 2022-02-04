using System.Threading.Tasks;
using AuthService.Models.Dtos;
using Common.Models.Dtos;

namespace AuthService.Services
{
    public interface IRegisterService
    {
        Task<ApplicationUserDto> RegisterAsync(RegisterRequestDto registerRequestDto);
    }
}
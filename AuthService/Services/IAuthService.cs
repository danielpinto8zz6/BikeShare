using System.Threading.Tasks;
using AuthService.Models.Dtos;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> AuthenticateAsync(AuthRequestDto authRequestDto);
    }
}
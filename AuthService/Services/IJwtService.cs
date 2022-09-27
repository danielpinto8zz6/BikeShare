using Common.Models.Dtos;

namespace AuthService.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserDto user);
    }
}
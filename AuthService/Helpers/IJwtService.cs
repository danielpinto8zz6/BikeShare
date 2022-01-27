using Common.Models.Dtos;

namespace AuthService.Helpers
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUserDto user);
    }
}
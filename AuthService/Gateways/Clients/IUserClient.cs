using System.Threading.Tasks;
using Common.Models.Dtos;
using Refit;

namespace AuthService.Gateways.Clients
{
    public interface IUserClient
    {
        [Get("/users/me")]
        Task<UserDto> GetByUsernameAsync([Header("UserId")] string username);
    }
}
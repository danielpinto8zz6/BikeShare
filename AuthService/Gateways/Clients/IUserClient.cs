using System.Threading.Tasks;
using Common.Models.Dtos;
using Refit;

namespace AuthService.Gateways.Clients
{
    public interface IUserClient
    {
        [Get("/users/{username}")]
        Task<ApplicationUserDto> GetByUsernameAsync(string username);

        [Post("/users")]
        Task<ApplicationUserDto> CreateAsync([Body] ApplicationUserDto applicationUserDto);
    }
}
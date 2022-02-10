using System.Threading.Tasks;
using TokenService.Models.Dto;

namespace TokenService.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenByKeyAsync(string key);

        Task PutTokenAsync(TokenRequestDto tokenRequestDto);
    }
}
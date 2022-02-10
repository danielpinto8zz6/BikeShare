using System.Threading.Tasks;

namespace NotificationService.Gateways;

public interface ITokenGateway
{
    public Task<string> GetTokenByKeyAsync(string key);
}
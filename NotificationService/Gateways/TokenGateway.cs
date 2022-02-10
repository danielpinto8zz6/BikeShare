using System.Threading.Tasks;
using NotificationService.Gateways.Client;

namespace NotificationService.Gateways;

public class TokenGateway : ITokenGateway
{
    private readonly ITokenClient _tokenClient;

    public TokenGateway(ITokenClient tokenClient)
    {
        _tokenClient = tokenClient;
    }

    public Task<string> GetTokenByKeyAsync(string key)
    {
        return _tokenClient.GetTokenByKeyAsync(key);
    }
}
using System.Threading.Tasks;
using Refit;

namespace NotificationService.Gateways.Client;

public interface ITokenClient
{
    [Get("/tokens/{key}")]
    public Task<string> GetTokenByKeyAsync(string key);
}
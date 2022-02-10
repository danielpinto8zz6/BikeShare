using System.Threading.Tasks;
using dotnet_etcd.interfaces;
using TokenService.Models.Dto;

namespace TokenService.Services
{
    public class TokenService : ITokenService
    {
        private readonly IEtcdClient _etcdClient;

        public TokenService(IEtcdClient etcdClient)
        {
            _etcdClient = etcdClient;
        }

        public async Task<string> GetTokenByKeyAsync(string key)
        {
            var token = await _etcdClient.GetValAsync(key);

            return token;
        }

        public Task PutTokenAsync(TokenRequestDto tokenRequestDto)
        {
            return _etcdClient.PutAsync(tokenRequestDto.Key, tokenRequestDto.Token);
        }
    }
}
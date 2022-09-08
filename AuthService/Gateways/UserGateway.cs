using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AuthService.Gateways.Clients;
using Common.Models.Dtos;
using Polly;
using Polly.CircuitBreaker;

namespace AuthService.Gateways
{
    public class UserGateway : IUserGateway
    {
        private readonly IUserClient _userClient;

        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        
        public UserGateway(IUserClient userClient, AsyncCircuitBreakerPolicy circuitBreaker)
        {
            _userClient = userClient;
            _circuitBreaker = circuitBreaker;
        }

        public async Task<UserDto> GetByUsernameAsync(string username)
        {
            //https://renatogroffe.medium.com/tratamento-de-falhas-com-net-polly-implementando-o-padrão-circuit-breaker-8727abcc7414
            var retry = Policy.Handle<HttpRequestException>(ex => ex.InnerException?.Message.Any() == true)
                .RetryAsync(5, async (exception, retryCount) =>
                    {
                        await Console.Out.WriteLineAsync("RetryPolicy execution...");
                    });


            var policy = Policy.WrapAsync(retry, _circuitBreaker);

            try
            {
                Console.WriteLine($"Circuit State: {_circuitBreaker.CircuitState}");

                return await policy.ExecuteAsync(() => _userClient.GetByUsernameAsync(username));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

            return null;
        }
    }
}
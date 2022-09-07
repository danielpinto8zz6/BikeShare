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
        
        public UserGateway(IUserClient userClient)
        {
            _userClient = userClient;
        }

        public async Task<UserDto> GetByUsernameAsync(string username)
        {
            var retry = Policy.Handle<HttpRequestException>(ex => ex.InnerException?.Message.Any() == true)
                .RetryAsync(5, async (exception, retryCount) =>
                    {
                        var lastColour = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        await Console.Out.WriteLineAsync("RetryPolicy execution...");
                        Console.ForegroundColor = lastColour;
                    });

            var circuitBreaker = GetCircuitBreaker();

            var policy = Policy.WrapAsync(retry, circuitBreaker);

            try
            {
                Console.WriteLine($"Circuit State: {circuitBreaker.CircuitState}");

                return await policy.ExecuteAsync(() => _userClient.GetByUsernameAsync(username));
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }

            return null;
        }
        
        public static AsyncCircuitBreakerPolicy GetCircuitBreaker()
        {
            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(10, TimeSpan.FromMinutes(1),
                    (ex, t) =>
                    {
                        Console.WriteLine("Circuit broken!");
                    },
                    () =>
                    {
                        Console.WriteLine("Circuit Reset!");
                    });

            return circuitBreakerPolicy;
        }
    }
}
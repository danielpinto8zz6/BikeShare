using AuthService.Gateways.Clients;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using Steeltoe.Common.Http.Discovery;

namespace AuthService.IntegrationTests;

public class AuthServiceTestApplication : WebApplicationFactory<Program>
{
    private readonly string _userServiceAddress;
    
    public AuthServiceTestApplication(string userServiceAddress)
    {
        _userServiceAddress = userServiceAddress;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddRefitClient<IUserClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(_userServiceAddress + "api"))
                .AddHttpMessageHandler<DiscoveryHttpMessageHandler>();
        });

        return base.CreateHost(builder);
    }
}
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace UserService.IntegrationTests;

public class UserServiceTestApplication : WebApplicationFactory<Program>
{
    private string MongoDbConnectionString;
    
    public UserServiceTestApplication(string mongoDbConnectionString)
    {
        MongoDbConnectionString = mongoDbConnectionString;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IMongoClient, MongoClient>(_ =>
                new MongoClient(MongoDbConnectionString));
        });

        return base.CreateHost(builder);
    }
}
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;

namespace UserService.IntegrationTests;

public class BaseMongoDbIntegrationTests
{
    protected MongoDbTestcontainer _mongoDbTestcontainer;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _mongoDbTestcontainer = new TestcontainersBuilder<MongoDbTestcontainer>()
            .WithImage("mongo:4.4")
            .WithCleanUp(true)
            .WithDatabase(new MongoDbTestcontainerConfiguration
            {
                Database = "user",
                Username = "username",
                Password = "password"
            })
            .Build();
        
        await _mongoDbTestcontainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _mongoDbTestcontainer.DisposeAsync();
    }
}
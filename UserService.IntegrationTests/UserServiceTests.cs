using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Common.Models.Dtos;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using NUnit.Framework;

namespace UserService.IntegrationTests;

public class UserServiceTests
{
    private MongoDbTestcontainer _mongoDbTestcontainer;

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

    [Test]
    public async Task CreateUser_WithUserRequest_ShouldReturnCreated()
    {
        var userDto = new UserDto
        {
            Name = "name",
            Password = "password",
            Username = Guid.NewGuid().ToString()
        };

        var application = new UserServiceTestApplication(_mongoDbTestcontainer.ConnectionString);
        var client = application.CreateClient();

        var payload = JsonSerializer.Serialize(userDto);
        var buffer = Encoding.UTF8.GetBytes(payload);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        var response = await client.PostAsync("/api/users", byteContent);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task GetUser_WithUserNameHeader_ShouldReturnOkWithUser()
    {
        var username = Guid.NewGuid().ToString();

        var userDto = new UserDto
        {
            Name = "name",
            Password = "password",
            Username = username
        };

        var application = new UserServiceTestApplication(_mongoDbTestcontainer.ConnectionString);
        var client = application.CreateClient();

        // Create user
        var payload = JsonSerializer.Serialize(userDto);
        var buffer = Encoding.UTF8.GetBytes(payload);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        await client.PostAsync("/api/users", byteContent);

        // Get user
        client.DefaultRequestHeaders.Add("UserId", username);
        var response = await client.GetAsync("/api/users/me");
        var responseValue = JsonSerializer.Deserialize<UserDto>(await response.Content.ReadAsStringAsync());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseValue.Should().NotBeNull();
    }
}
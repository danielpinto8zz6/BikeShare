using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Common.Models.Dtos;
using FluentAssertions;
using NUnit.Framework;

namespace UserService.IntegrationTests;

public class UserServiceTests : BaseMongoDbIntegrationTests
{
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
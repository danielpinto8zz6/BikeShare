using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuthService.Models.Dtos;
using Common.Models.Dtos;
using Common.Services;
using FluentAssertions;
using LightHTTP;
using NUnit.Framework;

namespace AuthService.IntegrationTests;

public class AuthServiceTests
{
    private LightHttpServer _httpServerMock;
    private HttpClient _client;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _httpServerMock = new LightHttpServer();
        var authServiceTestApplication = new AuthServiceTestApplication(_httpServerMock.AddAvailableLocalPrefix()); 
        _client = authServiceTestApplication.CreateClient();
        
        _httpServerMock.Start();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _httpServerMock.Stop();
        _client.Dispose();
    }

    [Test]
    public async Task Authenticate_WithValidUserAndPassword_ShouldReturnOk()
    {
        var username = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();

        var authRequest = new AuthRequestDto
        {
            Username = username,
            Password = password
        };

        var passwordService = new PasswordService();
        var userDto = new UserDto
        {
            Username = username,
            PasswordHash = passwordService.Hash(password)
        };


        _httpServerMock.HandlesPath("/api/users/me", async (context, cancellationToken) =>
        {
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;

            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userDto));
            await context.Response.OutputStream.WriteAsync(bytes, cancellationToken);
        });
        
        var authPayload = JsonSerializer.Serialize(authRequest);
        var authBuffer = Encoding.UTF8.GetBytes(authPayload);
        var authByteContent = new ByteArrayContent(authBuffer);
        authByteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        var authResponse = await _client.PostAsync("/api/auth", authByteContent);

        authResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
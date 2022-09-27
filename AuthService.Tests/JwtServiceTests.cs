using AuthService;
using AuthService.Services;
using Common.Models.Dtos;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace AuthServiceTests;

[TestFixture]
public class JwtServiceTests
{
    private IJwtService _jwtService;
    
    [SetUp]
    public void SetUp()
    {
        var appSettings = new AppSettings{Secret = Guid.NewGuid().ToString()};
        
        _jwtService = new JwtService(Options.Create(appSettings));
    }

    [Test]
    public void GenerateToken_WithValidUser_ShouldReturnToken()
    {
        var userDto = new UserDto
        {
            Id = "id",
            Name = "name",
            Password = "password",
            Username = "id"
        };

        var result = _jwtService.GenerateToken(userDto);

        result.Should().NotBeNull();
    }   
}
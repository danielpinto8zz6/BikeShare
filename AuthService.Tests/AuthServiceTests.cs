using AuthService.Models.Dtos;
using AuthService.Services;
using Common.Models.Dtos;
using Common.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AuthServiceTests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserService> _userServiceMock;
    private Mock<IPasswordService> _passwordServiceMock;
    private Mock<IJwtService> _jwtServiceMock;
    private IAuthService _authService;

    [SetUp]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _passwordServiceMock = new Mock<IPasswordService>();
        
        _authService = new AuthService.Services.AuthService(
            _userServiceMock.Object,
            _passwordServiceMock.Object,
            _jwtServiceMock.Object);
    }
    
    [Test]
    public async Task AuthenticateAsync_WithValidRequest_ShouldReturnToken()
    {
        var authRequestDto = new AuthRequestDto
        {
            Username = "username",
            Password = "password"
        };
        
        var token = Guid.NewGuid().ToString();
        
        _userServiceMock.Setup(i => i.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto());
        _passwordServiceMock.Setup(i => i.Matches(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        _jwtServiceMock.Setup(i => i.GenerateToken(It.IsAny<UserDto>()))
            .Returns(token);

        var result = await _authService.AuthenticateAsync(authRequestDto);

        result.Should().NotBeNull();
        result.Token.Should().Be(token);
    }

    [Test]
    public async Task AuthenticateAsync_WithInvalidUser_ShouldReturnNull()
    {
        var authRequestDto = new AuthRequestDto
        {
            Username = "username",
            Password = "password"
        };
        
        _userServiceMock.Setup(i => i.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync((UserDto) null);
        _passwordServiceMock.Setup(i => i.Matches(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        _jwtServiceMock.Setup(i => i.GenerateToken(It.IsAny<UserDto>()))
            .Returns(Guid.NewGuid().ToString);

        var result = await _authService.AuthenticateAsync(authRequestDto);

        result.Should().BeNull();
    }
    
    [Test]
    public async Task AuthenticateAsync_WithWrongPassword_ShouldReturnNull()
    {
        var authRequestDto = new AuthRequestDto
        {
            Username = "username",
            Password = "password"
        };
        
        _userServiceMock.Setup(i => i.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(new UserDto());
        _passwordServiceMock.Setup(i => i.Matches(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        _jwtServiceMock.Setup(i => i.GenerateToken(It.IsAny<UserDto>()))
            .Returns(Guid.NewGuid().ToString);

        var result = await _authService.AuthenticateAsync(authRequestDto);

        result.Should().BeNull();
    }
}
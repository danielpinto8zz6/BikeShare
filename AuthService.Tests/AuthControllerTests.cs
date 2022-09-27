using System.Net;
using AuthService.Controllers;
using AuthService.Models.Dtos;
using AuthService.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AuthService.Tests;

[TestFixture]
public class AuthControllerTests
{
    private AuthController _authController;
    private Mock<IAuthService> _authServiceMock;

    [SetUp]
    public void SetUp()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Test]
    public async Task AuthenticateAsync_WithValidRequest_ShouldReturnOk()
    {
        var request = new AuthRequestDto();
        _authServiceMock.Setup(i => i.AuthenticateAsync(It.IsAny<AuthRequestDto>()))
            .ReturnsAsync(new AuthResponseDto());

        var result = await _authController.AuthenticateAsync(request);

        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().StatusCode.Should().Be((int) HttpStatusCode.OK);
        result.As<OkObjectResult>().Value.Should().BeOfType<AuthResponseDto>();
    }

    [Test]
    public async Task AuthenticateAsync_WithInvalidRequest_ShouldReturnBadRequest()
    {
        var request = new AuthRequestDto();
        _authServiceMock.Setup(i => i.AuthenticateAsync(It.IsAny<AuthRequestDto>()))
            .ReturnsAsync((AuthResponseDto) null);

        var result = await _authController.AuthenticateAsync(request);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        result.As<BadRequestObjectResult>().StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
    }
}
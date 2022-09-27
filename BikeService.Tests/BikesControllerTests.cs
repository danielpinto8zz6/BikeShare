using System.Net;
using BikeService.Controllers;
using BikeService.Services;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BikeService.Tests;

public class BikesControllerTests
{
    private BikesController _bikesController;
    private Mock<IBikeService> _bikeServiceMock;

    [SetUp]
    public void SetUp()
    {
        _bikeServiceMock = new Mock<IBikeService>();
        _bikesController = new BikesController(_bikeServiceMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnOk()
    {
        _bikeServiceMock.Setup(i => i.GetAllAsync())
            .ReturnsAsync(new List<BikeDto>{new()});

        var result = await _bikesController.GetAllAsync();
        
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().StatusCode.Should().Be((int) HttpStatusCode.OK);
        result.As<OkObjectResult>().Value.Should().BeOfType<List<BikeDto>>();
    }
    
    [Test]
    public async Task GetByIdAsync_WithValidId_ShouldReturnOk()
    {
        _bikeServiceMock.Setup(i => i.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new BikeDto());

        var result = await _bikesController.GetByIdAsync(Guid.NewGuid());
        
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().StatusCode.Should().Be((int) HttpStatusCode.OK);
        result.As<OkObjectResult>().Value.Should().BeOfType<BikeDto>();
    }
    
    [Test]
    public async Task GetByIdAsync_WithNotFoundExceptionThrown_ShouldReturnNotFound()
    {
        _bikeServiceMock.Setup(i => i.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new NotFoundException());

        var result = await _bikesController.GetByIdAsync(Guid.NewGuid());
        
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
        result.As<NotFoundResult>().StatusCode.Should().Be((int) HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task CreateAsync_WithValidRequest_ShouldReturnCreated()
    {
        var request = new BikeDto();
        
        _bikeServiceMock.Setup(i => i.CreateAsync(It.IsAny<BikeDto>()))
            .ReturnsAsync(new BikeDto());

        var result = await _bikesController.CreateAsync(request);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtActionResult>();
        result.As<CreatedAtActionResult>().StatusCode.Should().Be((int) HttpStatusCode.Created);
        result.As<CreatedAtActionResult>().Value.Should().BeOfType<BikeDto>();
        result.As<CreatedAtActionResult>().ActionName.Should().Be("GetById");
    }
    
    [Test]
    public async Task UpdateAsync_WithValidRequest_ShouldReturnNoContent()
    {
        var bikeId = Guid.NewGuid();
        var request = new BikeDto();
        
        _bikeServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<BikeDto>()))
            .ReturnsAsync(new BikeDto());

        var result = await _bikesController.UpdateAsync(bikeId, request);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
        result.As<NoContentResult>().StatusCode.Should().Be((int) HttpStatusCode.NoContent);
    }
    
    [Test]
    public async Task UpdateAsync_WithNotFoundExceptionThrown_ShouldReturnNotFound()
    {
        var bikeId = Guid.NewGuid();
        var request = new BikeDto();
        
        _bikeServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<BikeDto>()))
            .ThrowsAsync(new NotFoundException());

        var result = await _bikesController.UpdateAsync(bikeId, request);
        
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();
        result.As<NotFoundResult>().StatusCode.Should().Be((int) HttpStatusCode.NotFound);
    }
}
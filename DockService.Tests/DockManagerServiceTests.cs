using System.Text.Json;
using AutoBogus;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using DockService.Consumers;
using DockService.Models.Dtos;
using DockService.Services;
using dotnet_etcd.interfaces;
using Etcdserverpb;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DockService.Tests;

public class DockManagerServiceTests
{
    private ITestHarness _testHarness;
    private IServiceProvider _provider;
    private Mock<IDockService> _dockServiceMock;
    private Mock<IEtcdClient> _etcdClientMock;
    private IDockManagerService _dockManagerService;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<BikeUnlockConsumer>(); })
            .AddLogging()
            .BuildServiceProvider(true);

        _testHarness = _provider.GetRequiredService<ITestHarness>();

        await _testHarness.Start();

        _etcdClientMock = new Mock<IEtcdClient>();
        var loggerMock = new Mock<ILogger<Services.DockService>>();

        _dockServiceMock = new Mock<IDockService>();

        _dockManagerService = new DockManagerService(
            _dockServiceMock.Object,
            _etcdClientMock.Object,
            _testHarness.Bus,
            loggerMock.Object);
    }

    [Test]
    public async Task UnlockBikeAsync_WithUnlockBikeRequest_ShouldSendBikeUnlocked()
    {
        var rentalDto = new AutoFaker<RentalDto>().Generate();
        var dockDto = new AutoFaker<DockDto>().Generate();
        
        _dockServiceMock.Setup(i => i.GetByBikeId(It.IsAny<Guid>()))
            .ReturnsAsync(dockDto);
        _dockServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DockDto>()))
            .ReturnsAsync(dockDto);
        _etcdClientMock.Setup(i => i.PutAsync(It.IsAny<string>(), It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new PutResponse());

        var rentalMessage = new RentalMessage
        {
            Rental = rentalDto,
            CorrelationId = Guid.NewGuid()
        };
        
        await _dockManagerService.UnlockBikeAsync(rentalMessage);
        
        (await _testHarness.Sent.Any<IBikeUnlocked>()).Should().BeTrue();
        (await _testHarness.Sent.Any<DockStateChangeRequest>()).Should().BeTrue();
    }
    
    [Test]
    public async Task UnlockBikeAsync_WithException_ShouldSendRentalFailure()
    {
        var rentalDto = new AutoFaker<RentalDto>().Generate();
        var dockDto = new AutoFaker<DockDto>().Generate();
        
        _dockServiceMock.Setup(i => i.GetByBikeId(It.IsAny<Guid>()))
            .ReturnsAsync(dockDto);
        _dockServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DockDto>()))
            .ReturnsAsync(dockDto);
        _etcdClientMock.Setup(i => i.PutAsync(It.IsAny<string>(), It.IsAny<string>(), null, null, default))
            .ThrowsAsync(new Exception());

        var rentalMessage = new RentalMessage
        {
            Rental = rentalDto,
            CorrelationId = Guid.NewGuid()
        };
        
        await _dockManagerService.UnlockBikeAsync(rentalMessage);
        
        (await _testHarness.Sent.Any<IRentalFailure>()).Should().BeTrue();
    }
    
    [Test]
    public async Task LockBikeAsync_WithBikeLockRequest_ShouldSendBikeLocked()
    {
        var rentalDto = new AutoFaker<RentalDto>().Generate();
        var dockDto = new AutoFaker<DockDto>().Generate();
        dockDto.BikeId = null;
        var bikeLockRequestDto = new BikeLockRequest
        {
            BikeId = Guid.NewGuid(),
            DockId = Guid.NewGuid()
        };
        var rentalMessage = new RentalMessage
        {
            Rental = rentalDto,
            CorrelationId = Guid.NewGuid()
        };
        
        _dockServiceMock.Setup(i => i.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dockDto);
        _dockServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DockDto>()))
            .ReturnsAsync(dockDto);
        _etcdClientMock.Setup(i => i.GetValAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(JsonSerializer.Serialize(rentalMessage));
        _etcdClientMock.Setup(i => i.DeleteAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new DeleteRangeResponse());

        await _dockManagerService.LockBikeAsync(bikeLockRequestDto);
        
        (await _testHarness.Sent.Any<IBikeLocked>()).Should().BeTrue();
        (await _testHarness.Sent.Any<DockStateChangeRequest>()).Should().BeTrue();
    }
    
    [Test]
    public async Task LockBikeAsync_WithBikeLockRequestOnOccupiedDock_ShouldThrowInvalidOperationException()
    {
        var rentalDto = new AutoFaker<RentalDto>().Generate();
        var dockDto = new AutoFaker<DockDto>().Generate();

        var bikeLockRequestDto = new BikeLockRequest
        {
            BikeId = Guid.NewGuid(),
            DockId = Guid.NewGuid()
        };
        var rentalMessage = new RentalMessage
        {
            Rental = rentalDto,
            CorrelationId = Guid.NewGuid()
        };
        
        _dockServiceMock.Setup(i => i.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dockDto);
        _dockServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DockDto>()))
            .ReturnsAsync(dockDto);
        _etcdClientMock.Setup(i => i.GetValAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(JsonSerializer.Serialize(rentalMessage));
        _etcdClientMock.Setup(i => i.DeleteAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new DeleteRangeResponse());
        
        Func<Task> f = async () => { await _dockManagerService.LockBikeAsync(bikeLockRequestDto); };
        await f.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"Dock with id: {dockDto.Id} already has a bike attached!");  
    }
}
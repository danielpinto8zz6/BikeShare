using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using DockService.Consumers;
using DockService.Models.Dtos;
using DockService.Services;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace DockService.Tests;

public class BikeReservationConsumerTests
{
    private ITestHarness _testHarness;
    
    private IServiceProvider _provider;

    private Mock<IDockService> _dockServiceMock;
    
    [SetUp]
    public async Task Setup()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<BikeReservationConsumer>();
            })
            .AddLogging()
            .AddScoped(_ => _dockServiceMock.Object)
            .BuildServiceProvider(true);

        _testHarness = _provider.GetRequiredService<ITestHarness>();

        await _testHarness.Start();

        _dockServiceMock = new Mock<IDockService>();
        _dockServiceMock.Setup(i => i.GetByBikeId(It.IsAny<Guid>()))
            .ReturnsAsync(new DockDto());
        _dockServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DockDto>()))
            .ReturnsAsync(new DockDto());
    }

    [Test]
    public async Task Consume_WithReserveBikeRequest_ShouldReplyWithBikeReserved()
    {
        var client = _testHarness.GetRequestClient<IReserveBike>();

        await client.GetResponse<IBikeReserved>(new
        {
            CorrelationId = Guid.NewGuid(),
            Rental  =new RentalDto()
        });

        (await _testHarness.Sent.Any<IBikeReserved>()).Should().BeTrue();

        (await _testHarness.Consumed.Any<IReserveBike>()).Should().BeTrue();

        var consumerHarness = _testHarness.GetConsumerHarness<BikeReservationConsumer>();

        (await consumerHarness.Consumed.Any<IReserveBike>()).Should().BeTrue();
    }
}
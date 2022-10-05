using BikeService.Consumers;
using BikeService.Services;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BikeService.Tests;

[TestFixture]
public class ValidateBikeConsumerTests
{
    private ValidateBikeConsumer _validateBikeConsumer;
    private Mock<IBikeService> _bikeService;
    private ITestHarness _testHarness;
    private IServiceProvider _provider;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _bikeService = new Mock<IBikeService>();
        
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<ValidateBikeConsumer>();
            })
            .AddLogging()
            .AddScoped(_ => _bikeService.Object)
            .BuildServiceProvider(true);

        _testHarness = _provider.GetRequiredService<ITestHarness>();

        await _testHarness.Start();
    }

    [Test]
    public async Task Consume_WithValidBike_ShouldSendBikeValidated()
    {
        _bikeService.Setup(i => i.ExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        
        await _testHarness.Bus.Publish<IValidateBike>(new
        {
            CorrelationId = Guid.NewGuid(),
            Rental = new RentalDto()
        });

        Thread.Sleep(200);
        
        (await _testHarness.Published.Any<IValidateBike>()).Should().BeTrue();
        (await _testHarness.Consumed.Any<IValidateBike>()).Should().BeTrue();

        var consumerHarness = _testHarness.GetConsumerHarness<ValidateBikeConsumer>();
        (await consumerHarness.Consumed.Any<IValidateBike>()).Should().BeTrue();

        (await _testHarness.Sent.Any<IBikeValidated>()).Should().BeTrue();
    }
    
    [Test]
    public async Task Consume_WithInvalidBike_ShouldSendRentalFailed()
    {
        _bikeService.Setup(i => i.ExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        
        await _testHarness.Bus.Publish<IValidateBike>(new
        {
            CorrelationId = Guid.NewGuid(),
            Rental = new RentalDto()
        });

        Thread.Sleep(200);
        
        (await _testHarness.Published.Any<IValidateBike>()).Should().BeTrue();
        (await _testHarness.Consumed.Any<IValidateBike>()).Should().BeTrue();

        var consumerHarness = _testHarness.GetConsumerHarness<ValidateBikeConsumer>();
        (await consumerHarness.Consumed.Any<IValidateBike>()).Should().BeTrue();

        (await _testHarness.Sent.Any<IRentalFailed>()).Should().BeTrue();
    }
}
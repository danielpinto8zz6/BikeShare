using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using FluentAssertions;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RentalService.Saga;
using RentalService.Services;

namespace RentalService.Tests;

[NonParallelizable]
public class RentalStateMachineTests
{
    private IServiceProvider _provider;

    private ITestHarness _testHarness;

    private Mock<IRentalService> _rentalServiceMock;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _rentalServiceMock = new Mock<IRentalService>();
        _rentalServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RentalDto>()))
            .ReturnsAsync(new RentalDto());

        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri("rabbitmq://192.168.1.199"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
                
                x.AddSagaStateMachine<RentalStateMachine, RentalState>();
            })
            .AddLogging()
            .AddTransient(_ => _rentalServiceMock.Object)
            .BuildServiceProvider(true);

        _testHarness = _provider.GetRequiredService<ITestHarness>();

        await _testHarness.Start();
    }
    
    [Test]
    public async Task RentalStateMachine_WhenRentalSubmitted_ShouldValidateBike()
    {
        var correlationId = Guid.NewGuid();
        var rental = new RentalDto
        {
            Id = Guid.NewGuid(),
            Status = RentalStatus.Submitted,
            Username = "username"
        };

        await _testHarness.Bus.Publish<IRentalSubmitted>(new
        {
            CorrelationId = correlationId,
            Rental = rental
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IRentalSubmitted>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<RentalStateMachine, RentalState>();

        (await sagaHarness.Consumed.Any<IRentalSubmitted>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Validating);

        instance.Should().NotBeNull();
        instance.Rental.Should().BeEquivalentTo(rental);

        (await _testHarness.Sent.Any<IValidateBike>()).Should().BeTrue();
    }
    
    [Test]
    public async Task RentalStateMachine_WhenBikeValidated_ShouldUnlockBike()
    {
        var correlationId = Guid.NewGuid();
        var rental = new RentalDto
        {
            Id = Guid.NewGuid(),
            Status = RentalStatus.BikeValidated,
            Username = "username"
        };

        await _testHarness.Bus.Publish<IRentalSubmitted>(new
        {
            CorrelationId = correlationId,
            Rental = rental
        });
        
        Thread.Sleep(200);

        await _testHarness.Bus.Publish<IBikeValidated>(new
        {
            CorrelationId = correlationId,
            Rental = rental
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IBikeValidated>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<RentalStateMachine, RentalState>();

        (await sagaHarness.Consumed.Any<IBikeValidated>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Unlocking);

        instance.Should().NotBeNull();
        instance.Rental.Should().BeEquivalentTo(rental);

        (await _testHarness.Sent.Any<IUnlockBike>()).Should().BeTrue();
    }
}
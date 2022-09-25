using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RentalService.Saga;
using RentalService.Services;

namespace RentalService.Tests;

public class RentalStateMachineTests
{
    private IServiceProvider _provider;

    private ITestHarness _testHarness;

    private Mock<IRentalService> _rentalServiceMock;

    private EndpointResolver _endpointResolverMock;

    [SetUp]
    public async Task Setup()
    {
        _rentalServiceMock = new Mock<IRentalService>();
        _rentalServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RentalDto>()))
            .ReturnsAsync(new RentalDto());
        _endpointResolverMock = new EndpointResolver("");

        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<RentalStateMachine, RentalState>()
                    .InMemoryRepository();
                
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .AddLogging()
            .AddSingleton(_ => _endpointResolverMock)
            .AddTransient(_ => _rentalServiceMock.Object)
            .BuildServiceProvider(true);

        _testHarness = _provider.GetRequiredService<ITestHarness>();

        await _testHarness.Start();
    }

    [TearDown]
    public async Task TearDown()
    {
    }

    [Test]
    public async Task Test1()
    {
        var sagaId = Guid.NewGuid();
        var rental = new RentalDto
        {
            Id = Guid.NewGuid(),
            Status = RentalStatus.Submitted,
            Username = "username"
        };

        await _testHarness.Bus.Publish<IRentalSubmitted>(new
        {
            CorrelationId = sagaId,
            Rental = rental
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IRentalSubmitted>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<RentalStateMachine, RentalState>();

        (await sagaHarness.Consumed.Any<IRentalSubmitted>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == sagaId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(sagaId, sagaHarness.StateMachine, sagaHarness.StateMachine.Validating);

        instance.Should().NotBeNull();
        instance.Rental.Should().BeEquivalentTo(rental);

        (await _testHarness.Published.Any<IValidateBike>()).Should().BeTrue();
    }
}
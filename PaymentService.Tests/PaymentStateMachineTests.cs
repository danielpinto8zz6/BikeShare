using Common.Models.Commands.Payment;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using PaymentService.Saga;
using PaymentService.Services;

namespace PaymentService.Tests;

[NonParallelizable]
public class PaymentStateMachineTests
{
    private IServiceProvider _provider;

    private ITestHarness _testHarness;

    private Mock<IPaymentService> _paymentServiceMock;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _paymentServiceMock = new Mock<IPaymentService>();

        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<PaymentStateMachine, PaymentState>();
            })
            .AddLogging()
            .AddTransient(_ => _paymentServiceMock.Object)
            .BuildServiceProvider(true);

        _testHarness = _provider.GetRequiredService<ITestHarness>();

        await _testHarness.Start();
    }
    
    [Test]
    public async Task PaymentStateMachine_WhenPaymentRequested_ShouldCalculatePayment()
    {
        var correlationId = Guid.NewGuid();
        var payment = new PaymentDto
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Requested,
            Username = "username",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMinutes(10),
            RentalId = Guid.NewGuid()
        };
        
        _paymentServiceMock.Setup(i => i.CreateAsync(It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);

        await _testHarness.Bus.Publish<IPaymentRequested>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IPaymentRequested>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<PaymentStateMachine, PaymentState>();

        (await sagaHarness.Consumed.Any<IPaymentRequested>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Calculating);

        instance.Should().NotBeNull();

        (await _testHarness.Sent.Any<ICalculatePayment>()).Should().BeTrue();
    }
    
    [Test]
    public async Task PaymentStateMachine_WhenPaymentCalculated_ShouldValidatePayment()
    {
        var correlationId = Guid.NewGuid();
        var payment = new PaymentDto
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Requested,
            Username = "username",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMinutes(10),
            RentalId = Guid.NewGuid()
        };

        _paymentServiceMock.Setup(i => i.CreateAsync(It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        _paymentServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        
        await _testHarness.Bus.Publish<IPaymentRequested>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });
        
        Thread.Sleep(200);

        await _testHarness.Bus.Publish<IPaymentCalculated>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IPaymentCalculated>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<PaymentStateMachine, PaymentState>();

        (await sagaHarness.Consumed.Any<IPaymentCalculated>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Validating);

        instance.Should().NotBeNull();

        (await _testHarness.Sent.Any<IValidatePayment>()).Should().BeTrue();
    }
    
    [Test]
    public async Task PaymentStateMachine_WhenPaymentCalculationFailed_ShouldFailed()
    {
        var correlationId = Guid.NewGuid();
        var payment = new PaymentDto
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Requested,
            Username = "username",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMinutes(10),
            RentalId = Guid.NewGuid()
        };

        _paymentServiceMock.Setup(i => i.CreateAsync(It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        _paymentServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        
        await _testHarness.Bus.Publish<IPaymentRequested>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });
        
        Thread.Sleep(200);

        await _testHarness.Bus.Publish<IPaymentCalculationFailed>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IPaymentCalculationFailed>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<PaymentStateMachine, PaymentState>();

        (await sagaHarness.Consumed.Any<IPaymentCalculationFailed>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Final);

        instance.Should().NotBeNull();
    }

    [Test]
    public async Task PaymentStateMachine_WhenPaymentValidated_ShouldFinalize()
    {
        var correlationId = Guid.NewGuid();
        var payment = new PaymentDto
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Requested,
            Username = "username",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMinutes(10),
            RentalId = Guid.NewGuid()
        };

        _paymentServiceMock.Setup(i => i.CreateAsync(It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        _paymentServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        
        await _testHarness.Bus.Publish<IPaymentRequested>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });
        
        Thread.Sleep(200);

        await _testHarness.Bus.Publish<IPaymentCalculated>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);
        
        await _testHarness.Bus.Publish<IPaymentValidated>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IPaymentValidated>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<PaymentStateMachine, PaymentState>();

        (await sagaHarness.Consumed.Any<IPaymentValidated>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Final);

        instance.Should().NotBeNull();
    }
    
    [Test]
    public async Task PaymentStateMachine_WhenPaymentValidationFailed_ShouldFailed()
    {
        var correlationId = Guid.NewGuid();
        var payment = new PaymentDto
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Requested,
            Username = "username",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMinutes(10),
            RentalId = Guid.NewGuid()
        };

        _paymentServiceMock.Setup(i => i.CreateAsync(It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        _paymentServiceMock.Setup(i => i.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PaymentDto>()))
            .ReturnsAsync(payment);
        
        await _testHarness.Bus.Publish<IPaymentRequested>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });
        
        Thread.Sleep(200);

        await _testHarness.Bus.Publish<IPaymentCalculated>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);
        
        await _testHarness.Bus.Publish<IPaymentValidationFailed>(new
        {
            CorrelationId = correlationId,
            Payment = payment
        });

        Thread.Sleep(200);

        (await _testHarness.Consumed.Any<IPaymentValidationFailed>()).Should().BeTrue();

        var sagaHarness = _testHarness.GetSagaStateMachineHarness<PaymentStateMachine, PaymentState>();

        (await sagaHarness.Consumed.Any<IPaymentValidationFailed>()).Should().BeTrue();

        (await sagaHarness.Created.Any(x => x.CorrelationId == correlationId)).Should().BeTrue();

        var instance =
            sagaHarness.Created.ContainsInState(correlationId, sagaHarness.StateMachine, sagaHarness.StateMachine.Final);

        instance.Should().NotBeNull();
    }
}
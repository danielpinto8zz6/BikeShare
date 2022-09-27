using System;
using System.Threading.Tasks;
using Common.Models.Commands.Payment;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaymentService.Helpers;
using PaymentService.Saga;

namespace PaymentService.Services;

public sealed class PaymentStateMachine : MassTransitStateMachine<PaymentState>
{
    private readonly ILogger<PaymentStateMachine> _logger;

    private readonly IServiceProvider _serviceProvider;

    public PaymentStateMachine(
        ILogger<PaymentStateMachine> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        InstanceState(x => x.Status);

        State(() => Validating);

        ConfigureCorrelationIds();

        Initially(SetPaymentRequestedHandler());
        During(Calculating, SetPaymentCalculatedHandler(), SetPaymentCalculationFailedHandler());
        During(Validating, SetPaymentValidatedHandler(), SetPaymentValidationFailedHandler());

        SetCompletedWhenFinalized();
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => Requested, x => x.CorrelateById(c => c.Message.CorrelationId)
            .SelectId(c => c.Message.CorrelationId));
        Event(() => Calculated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => CalculationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => Validated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => ValidationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<PaymentState, IPaymentRequested> SetPaymentRequestedHandler() =>
        When(Requested)
            .ThenAsync(CreatePayment)
            .Then(c => _logger.LogInformation($"Payment requested to {c.Message.CorrelationId} received"))
            .SendAsync(new Uri($"queue:{nameof(ICalculatePayment)}"), BuildCommand<ICalculatePayment>)
            .TransitionTo(Calculating);
    
    private EventActivityBinder<PaymentState, IPaymentCalculated> SetPaymentCalculatedHandler() =>
        When(Calculated)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Calculated))
            .Then(c => _logger.LogInformation($"Payment calculated to {c.Message.CorrelationId} received"))
            .SendAsync(new Uri($"queue:{nameof(IValidatePayment)}"), BuildCommand<IValidatePayment>)
            .TransitionTo(Validating);

    private EventActivityBinder<PaymentState, IPaymentCalculationFailed> SetPaymentCalculationFailedHandler() =>
        When(CalculationFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.CalculationFailed))
            .Then(c => _logger.LogInformation($"Payment calculated to {c.Message.CorrelationId} received"))
            .Finalize();

    private EventActivityBinder<PaymentState, IPaymentValidated> SetPaymentValidatedHandler() =>
        When(Validated)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Validated))
            .Then(c => _logger.LogInformation($"Payment validated to {c.Message.CorrelationId} received"))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetPaymentSucceedNotification(c)))
            .TransitionTo(Completed)
            .Finalize();

    private EventActivityBinder<PaymentState, IPaymentValidationFailed> SetPaymentValidationFailedHandler() =>
        When(ValidationFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.ValidationFailed))
            .Then(c => _logger.LogInformation($"Payment validation failed to {c.Message.CorrelationId} received"))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetPaymentFailedNotification(c)))
            .Finalize();

    private async Task UpdateSagaState(PaymentState state, PaymentDto payment, PaymentStatus paymentStatus)
    {
        var currentDate = DateTime.UtcNow;

        payment.Status = paymentStatus;
        state.Created = currentDate;
        state.Updated = currentDate;
        state.Status = (int) paymentStatus;
        state.Payment = payment;

        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
    
        await paymentService.UpdateAsync(payment.Id, payment);
    }
    
    private async Task CreatePayment(BehaviorContext<PaymentState, IPaymentRequested> context)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

        context.Message.Payment.Status = PaymentStatus.Requested;
        
        var paymentDto = await paymentService.CreateAsync(context.Message.Payment);

        context.Message.Payment.Id = paymentDto.Id;
    }

    private Task<SendTuple<T>> BuildCommand<T>(BehaviorContext<PaymentState, IPaymentMessage> context) where T : class
    {
        return context.Init<T>(new
        {
            CorrelationId = context.CorrelationId ?? Guid.Empty,
            Payment = context.Message.Payment
        });
    }

    public State Calculating { get; private set; }
    public State Validating { get; private set; }
    public State Completed { get; private set; }

    public Event<IPaymentRequested> Requested { get; private set; }
    public Event<IPaymentCalculated> Calculated { get; private set; }
    public Event<IPaymentCalculationFailed> CalculationFailed { get; private set; }
    public Event<IPaymentValidated> Validated { get; private set; }
    public Event<IPaymentValidationFailed> ValidationFailed { get; private set; }
}
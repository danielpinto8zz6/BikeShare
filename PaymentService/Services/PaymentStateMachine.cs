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
        Event(() => Validated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => CalculationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => ValidationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<PaymentState, IPaymentRequested> SetPaymentRequestedHandler() =>
        When(Requested)
            .ThenAsync(CreatePaymentAsync)
            .SendAsync(new Uri($"queue:{nameof(ICalculatePayment)}"), BuildCommand<ICalculatePayment>)
            .TransitionTo(Calculating)
            .Then(c => _logger.LogInformation($"Payment requested to {c.Message.CorrelationId} received"));

    private EventActivityBinder<PaymentState, IPaymentCalculated> SetPaymentCalculatedHandler() =>
        When(Calculated)
            .Then(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Calculated))
            .SendAsync(new Uri($"queue:{nameof(IValidatePayment)}"), BuildCommand<IValidatePayment>)
            .TransitionTo(Validating)
            .Then(c => _logger.LogInformation($"Payment calculated to {c.Message.CorrelationId} received"));

    private EventActivityBinder<PaymentState, IPaymentValidated> SetPaymentValidatedHandler() =>
        When(Validated)
            .Then(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Validated))
            .ThenAsync(c => UpdatePaymentAsync(c.Message.Payment))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetPaymentSucceedNotification(c)))
            .Finalize()
            .Then(c => _logger.LogInformation($"Payment validated to {c.Message.CorrelationId} received"));

    private EventActivityBinder<PaymentState, IPaymentCalculationFailed> SetPaymentCalculationFailedHandler() =>
        When(CalculationFailed)
            .Then(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.CalculationFailed))
            .ThenAsync(c => UpdatePaymentAsync(c.Message.Payment))
            .Finalize()
            .Then(c => _logger.LogInformation($"Payment calculated to {c.Message.CorrelationId} received"));

    private EventActivityBinder<PaymentState, IPaymentValidationFailed> SetPaymentValidationFailedHandler() =>
        When(ValidationFailed)
            .Then(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.ValidationFailed))
            .ThenAsync(c => UpdatePaymentAsync(c.Message.Payment))
            .Finalize()
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetPaymentFailedNotification(c)))
            .Then(c => _logger.LogInformation($"Payment validation failed to {c.Message.CorrelationId} received"));

    private void UpdateSagaState(PaymentState state, PaymentDto payment, PaymentStatus paymentStatus)
    {
        var currentDate = DateTime.UtcNow;

        payment.Status = paymentStatus;
        state.Updated = currentDate;
        state.Status = (int) paymentStatus;
        state.Payment = payment;
        if (paymentStatus == PaymentStatus.Requested)
        {
            state.Created = currentDate;
        }
    }
    
    private Task<PaymentDto> UpdatePaymentAsync(PaymentDto payment)
    {
        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

        return paymentService.UpdateAsync(payment.Id, payment);
    }

    private async Task CreatePaymentAsync(BehaviorContext<PaymentState, IPaymentRequested> context)
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

    public Event<IPaymentRequested> Requested { get; private set; }
    public Event<IPaymentCalculated> Calculated { get; private set; }
    public Event<IPaymentCalculationFailed> CalculationFailed { get; private set; }
    public Event<IPaymentValidated> Validated { get; private set; }
    public Event<IPaymentValidationFailed> ValidationFailed { get; private set; }
}
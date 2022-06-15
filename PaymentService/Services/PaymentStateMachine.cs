using System;
using System.Threading.Tasks;
using Common.Models.Commands.Payment;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
using LSG.GenericCrud.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        During(Calculating,
            SetPaymentCalculatedHandler(),
            SetPaymentCalculationFailedHandler()
        );
        
        During(Validating,
            SetPaymentValidatedHandler(),
            SetPaymentValidationFailedHandler()
        );

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
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Requested))
            .Then(c => _logger.LogInformation($"Payment requested to {c.Message.CorrelationId} received"))
            .ThenAsync(c => SendCommand<ICalculatePayment>("rabbitmq://192.168.1.199/payment-calculate", c))
            .TransitionTo(Calculating);

    private EventActivityBinder<PaymentState, IPaymentCalculated> SetPaymentCalculatedHandler() =>
        When(Calculated)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Calculated))
            .Then(c => _logger.LogInformation($"Payment calculated to {c.Message.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IValidatePayment>("rabbitmq://192.168.1.199/payment-validate", c))
            .TransitionTo(Validating);

    private EventActivityBinder<PaymentState, IPaymentCalculationFailed> SetPaymentCalculationFailedHandler() =>
        When(CalculationFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.CalculationFailed))
            .Then(c => _logger.LogInformation($"Payment calculated to {c.Message.CorrelationId} received"))
            .TransitionTo(Failed);
    
    private EventActivityBinder<PaymentState, IPaymentValidated> SetPaymentValidatedHandler() =>
        When(Validated)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.Validated))
            .Then(c => _logger.LogInformation($"Payment validated to {c.Message.CorrelationId} received"))
            .TransitionTo(Completed);

    private EventActivityBinder<PaymentState, IPaymentValidationFailed> SetPaymentValidationFailedHandler() =>
        When(ValidationFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Payment, PaymentStatus.ValidationFailed))
            .Then(c => _logger.LogInformation($"Payment validation failed to {c.Message.CorrelationId} received"))
            .TransitionTo(Failed);
    
    private async Task UpdateSagaState(PaymentState state, PaymentDto payment, PaymentStatus paymentStatus)
    {
        var currentDate = DateTime.UtcNow;

        payment.Status = paymentStatus;
        state.Created = currentDate;
        state.Updated = currentDate;
        state.Status = (int) paymentStatus;
        state.Payment = payment;

        using var scope = _serviceProvider.CreateScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<ICrudService<Guid, PaymentDto>>();

        await paymentService.UpdateAsync(payment.Id, payment);
    }

    private static async Task SendCommand<TCommand>(string endpoint,
        ConsumeContext<IPaymentMessage> context) where TCommand : class, IPaymentMessage
    {
        var sendEndpoint = await context.GetSendEndpoint(new Uri(endpoint));

        await sendEndpoint.Send<TCommand>(new
        {
            context.CorrelationId,
            context.Message.Payment
        });
    }

    public State Calculating { get; private set; }

    public State Validating { get; private set; }

    public State Failed { get; private set; }
    
    public State Completed { get; private set; }

    public Event<IPaymentRequested> Requested { get; private set; }

    public Event<IPaymentCalculated> Calculated { get; private set; }

    public Event<IPaymentCalculationFailed> CalculationFailed { get; private set; }

    public Event<IPaymentValidated> Validated { get; private set; }

    public Event<IPaymentValidationFailed> ValidationFailed { get; private set; }
}
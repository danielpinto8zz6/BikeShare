using System;
using System.Threading.Tasks;
using Common.Models.Commands;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events;
using Common.Models.Events.Rental;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RentalService.Helpers;
using RentalService.Saga;

namespace RentalService.Services;

public sealed class RentalStateMachine : MassTransitStateMachine<RentalState>
{
    private readonly ILogger<RentalStateMachine> _logger;

    private readonly IServiceProvider _serviceProvider;

    public RentalStateMachine(
        ILogger<RentalStateMachine> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        InstanceState(x => x.Status);

        State(() => Validating);

        ConfigureCorrelationIds();

        Initially(SetRentalSummitedHandler());

        During(Validating,
            SetBikeValidatedHandler(),
            SetBikeValidationFailedHandler()
        );

        During(Reserving,
            SetBikeReservedHandler(),
            SetBikeReservationFailedHandler()
        );

        During(Unlocking,
            SetBikeUnlockedHandler(),
            SetBikeUnlockFailedHandler()
        );

        During(InUse,
            SetBikeLockedHandler(),
            SetBikeLockFailedHandler()
        );

        During(Locking,
            SetBikeAttachedHandler(),
            SetBikeAttachFailedHandler()
        );

        During(ProcessingPayment,
            SetPaymentCompletedHandler(),
            SetPaymentFailedHandler()
        );

        SetCompletedWhenFinalized();
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => RentalSubmitted, x => x.CorrelateById(c => c.Message.CorrelationId)
            .SelectId(c => c.Message.CorrelationId));
        Event(() => BikeValidated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeReserved, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlocked, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeLocked, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeAttached, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => PaymentCompleted, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeValidationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeReservationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlockFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeLockFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeAttachFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => PaymentFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<RentalState, IRentalSubmitted> SetRentalSummitedHandler() =>
        When(RentalSubmitted)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.Submitted))
            .Then(c => _logger.LogInformation($"Rental submitted to {c.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IUnlockBike>("rabbitmq://192.168.1.199/bike-unlock", c))
            .ThenAsync(c => SendCommand<IValidateBike>("rabbitmq://192.168.1.199/bike-validate", c))
            .TransitionTo(Unlocking);

    private EventActivityBinder<RentalState, IBikeValidated> SetBikeValidatedHandler() =>
        When(BikeValidated)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeValidated))
            .Then(c => _logger.LogInformation($"Bike validated to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeValidatedNotificationAsync)
            .ThenAsync(c => SendCommand<IReserveBike>("rabbitmq://192.168.1.199/bike-reservation", c))
            .TransitionTo(Reserving);

    private EventActivityBinder<RentalState, IBikeReserved> SetBikeReservedHandler() =>
        When(BikeReserved)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeReserved))
            .Then(c => _logger.LogInformation($"Bike reserved to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeReservedNotificationAsync)
            .ThenAsync(c => SendCommand<IUnlockBike>("rabbitmq://192.168.1.199/bike-unlock", c))
            .TransitionTo(Unlocking);

    private EventActivityBinder<RentalState, IBikeUnlocked> SetBikeUnlockedHandler() =>
        When(BikeUnlocked)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeUnlocked))
            .Then(c => _logger.LogInformation($"Bike unlock to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeUnlockedNotificationAsync)
            .TransitionTo(InUse);

    private EventActivityBinder<RentalState, IBikeLocked> SetBikeLockedHandler() =>
        When(BikeLocked)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeLocked))
            .Then(c => _logger.LogInformation($"Bike locked to {c.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IAttachBike>("rabbitmq://192.168.1.199/bike-unlock", c))
            .TransitionTo(Locking);

    private EventActivityBinder<RentalState, IBikeUnlockFailed> SetBikeUnlockFailedHandler() =>
        When(BikeUnlockFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeUnlockFailed))
            .Then(c => _logger.LogInformation($"Bike unlock failed to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeUnlockFailedNotificationAsync)
            .TransitionTo(Failed);

    private EventActivityBinder<RentalState, IBikeAttached> SetBikeAttachedHandler() =>
        When(BikeAttached)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeAttached))
            .Then(c => _logger.LogInformation($"Bike attached to {c.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IProcessPayment>("rabbitmq://192.168.1.199/payment-process", c))
            .ThenAsync(NotificationHelper.SendBikeAttachedNotificationAsync)
            .TransitionTo(ProcessingPayment);

    private EventActivityBinder<RentalState, IPaymentCompleted> SetPaymentCompletedHandler() =>
        When(PaymentCompleted)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.PaymentCompleted))
            .Then(c => _logger.LogInformation($"Payment completed to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendPaymentCompletedNotificationAsync)
            .TransitionTo(Completed);

    private EventActivityBinder<RentalState, IPaymentFailed> SetPaymentFailedHandler() =>
        When(PaymentFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.PaymentFailed))
            .Then(c => _logger.LogInformation($"Payment failed to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendPaymentCompletedNotificationAsync)
            .TransitionTo(Failed);

    private EventActivityBinder<RentalState, IBikeLockFailed> SetBikeLockFailedHandler() =>
        When(BikeLockFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeLockFailed))
            .Then(c => _logger.LogInformation($"Bike lock failed to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeLockFailedNotificationAsync)
            .TransitionTo(Failed);

    private EventActivityBinder<RentalState, IBikeValidationFailed> SetBikeValidationFailedHandler() =>
        When(BikeValidationFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeValidationFailed))
            .Then(c => _logger.LogInformation($"Bike validation failed to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeValidationFailedNotificationAsync)
            .TransitionTo(Failed);

    private EventActivityBinder<RentalState, IBikeReservationFailed> SetBikeReservationFailedHandler() =>
        When(BikeReservationFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeReservationFailed))
            .Then(c => _logger.LogInformation($"Bike reservation failed to {c.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeReservationFailedNotificationAsync)
            .TransitionTo(Failed);

    private EventActivityBinder<RentalState, IBikeAttachFailed> SetBikeAttachFailedHandler() =>
        When(BikeAttachFailed)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeAttachFailed))
            .Then(c => _logger.LogInformation($"Bike attach failed to {c.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IAttachBike>("rabbitmq://192.168.1.199/bike-attach", c))
            .ThenAsync(c => SendCommand<IProcessPayment>("rabbitmq://192.168.1.199/payment-process", c))
            .TransitionTo(Failed);

    private async Task UpdateSagaState(RentalState state, RentalDto rental, RentalStatus rentalStatus)
    {
        var currentDate = DateTime.UtcNow;

        if (rentalStatus == RentalStatus.BikeUnlocked)
            rental.StartDate = currentDate;
        else if (rentalStatus == RentalStatus.BikeLocked) 
            rental.EndDate = currentDate;

        rental.Status = rentalStatus;
        state.Created = currentDate;
        state.Updated = currentDate;
        state.Status = (int) rentalStatus;
        state.Rental = rental;

        using var scope = _serviceProvider.CreateScope();
        var rentalService = scope.ServiceProvider.GetRequiredService<IRentalService>();

        await rentalService.UpdateAsync(rental.Id, rental);
    }

    private static async Task SendCommand<TCommand>(string endpoint,
        BehaviorContext<RentalState, IRentalMessage> context) where TCommand : class, IRentalMessage
    {
        var sendEndpoint = await context.GetSendEndpoint(new Uri(endpoint));

        await sendEndpoint.Send<TCommand>(new
        {
            context.CorrelationId,
            context.Message.Rental
        });
    }

    public State Validating { get; private set; }

    public State Reserving { get; private set; }

    public State Unlocking { get; private set; }

    public State Locking { get; private set; }

    public State ProcessingPayment { get; private set; }

    public State Completed { get; private set; }

    public State InUse { get; private set; }

    public State Failed { get; private set; }

    public Event<IRentalSubmitted> RentalSubmitted { get; private set; }

    public Event<IBikeReserved> BikeReserved { get; private set; }

    public Event<IBikeValidated> BikeValidated { get; private set; }

    public Event<IBikeUnlocked> BikeUnlocked { get; private set; }

    public Event<IBikeLocked> BikeLocked { get; private set; }

    public Event<IBikeAttached> BikeAttached { get; private set; }

    public Event<IPaymentCompleted> PaymentCompleted { get; private set; }

    public Event<IBikeValidationFailed> BikeValidationFailed { get; private set; }

    public Event<IBikeReservationFailed> BikeReservationFailed { get; private set; }

    public Event<IBikeUnlockFailed> BikeUnlockFailed { get; private set; }

    public Event<IBikeLockFailed> BikeLockFailed { get; private set; }

    public Event<IBikeAttachFailed> BikeAttachFailed { get; private set; }

    public Event<IPaymentFailed> PaymentFailed { get; private set; }
}
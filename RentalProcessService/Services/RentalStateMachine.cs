using Common.Models.Commands;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events;
using MassTransit;
using RentalProcessService.Helpers;
using RentalProcessService.Saga;

namespace RentalProcessService.Services;

public sealed class RentalStateMachine : MassTransitStateMachine<RentalState>
{
    private readonly ILogger<RentalStateMachine> _logger;

    public RentalStateMachine(
        ILogger<RentalStateMachine> logger)
    {
        _logger = logger;

        InstanceState(x => x.Status);
        State(() => Validating);
        ConfigureCorrelationIds();
        Initially(SetRentalSummitedHandler());
        During(Validating,
            SetBikeValidatedHandler(),
            SetBikeValidationFailedHandler());
        During(Reserving,
            SetBikeReservedHandler(),
            SetBikeReservationFailedHandler()
        );
        During(Unlocking,
            SetBikeUnlockedHandler(),
            SetBikeUnlockFailedHandler());
        SetCompletedWhenFinalized();
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => RentalSubmitted, x => x.CorrelateById(c => c.Message.CorrelationId)
            .SelectId(c => c.Message.CorrelationId));
        
        Event(() => BikeValidated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeReserved, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlocked, x => x.CorrelateById(c => c.Message.CorrelationId));
        
        Event(() => BikeValidationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeReservationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlockFailed, x => x.CorrelateById(c => c.Message.CorrelationId));

    }

    private EventActivityBinder<RentalState, IRentalSubmitted> SetRentalSummitedHandler() =>
        When(RentalSubmitted)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.Submitted))
            .Then(c => _logger.LogInformation($"Rental submitted to {c.Data.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IValidateBike>("rabbitmq://192.168.1.199/bike-validate", c))
            .TransitionTo(Validating);

    private EventActivityBinder<RentalState, IBikeValidated> SetBikeValidatedHandler() =>
        When(BikeValidated)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.BikeValidated))
            .Then(c => _logger.LogInformation($"Bike validated to {c.Data.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeValidatedNotificationAsync)
            .ThenAsync(c => SendCommand<IReserveBike>("rabbitmq://192.168.1.199/bike-reservation", c))
            .TransitionTo(Reserving);

    private EventActivityBinder<RentalState, IBikeReserved> SetBikeReservedHandler() =>
        When(BikeReserved)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.BikeReserved))
            .Then(c => _logger.LogInformation($"Bike reserved to {c.Data.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeReservedNotificationAsync)
            .ThenAsync(c => SendCommand<IUnlockBike>("rabbitmq://192.168.1.199/bike-unlock", c))
            .TransitionTo(Unlocking);

    private EventActivityBinder<RentalState, IBikeUnlocked> SetBikeUnlockedHandler() =>
        When(BikeUnlocked)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.BikeUnlocked))
            .Then(c => _logger.LogInformation($"Bike unlock to {c.Data.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeUnlockedNotificationAsync)
            .TransitionTo(InUse);
    
    private EventActivityBinder<RentalState, IBikeUnlockFailed> SetBikeUnlockFailedHandler() =>
        When(BikeUnlockFailed)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.BikeUnlockFailed))
            .Then(c => _logger.LogInformation($"Bike unlock failed to {c.Data.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeUnlockFailedNotificationAsync)
            .TransitionTo(Failed);
    
    private EventActivityBinder<RentalState, IBikeValidationFailed> SetBikeValidationFailedHandler() =>
        When(BikeValidationFailed)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.BikeValidationFailed))
            .Then(c => _logger.LogInformation($"Bike validation failed to {c.Data.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeValidationFailedNotificationAsync)
            .TransitionTo(Failed);

    private EventActivityBinder<RentalState, IBikeReservationFailed> SetBikeReservationFailedHandler() =>
        When(BikeReservationFailed)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental, RentalStatus.BikeReservationFailed))
            .Then(c => _logger.LogInformation($"Bike reservation failed to {c.Data.CorrelationId} received"))
            .ThenAsync(NotificationHelper.SendBikeReservationFailedNotificationAsync)
            .TransitionTo(Failed);

    private static void UpdateSagaState(RentalState state, RentalDto rental, RentalStatus rentalStatus)
    {
        var currentDate = DateTime.UtcNow;

        state.Created = currentDate;
        state.Updated = currentDate;
        state.Status = (int) rentalStatus;
        state.Rental = rental;
    }

    private static async Task SendCommand<TCommand>(string endpoint,
        BehaviorContext<RentalState, IRentalMessage> context) where TCommand : class, IRentalMessage
    {
        var sendEndpoint = await context.GetSendEndpoint(new Uri(endpoint));

        await sendEndpoint.Send<TCommand>(new
        {
            context.Data.CorrelationId,
            context.Data.Rental
        });
    }

    public State Validating { get; private set; }
    
    public State Reserving { get; private set; }
    
    public State Unlocking { get; private set; }
    
    public State InUse { get; private set; }
    
    public State Failed { get; private set; }

    public Event<IRentalSubmitted> RentalSubmitted { get; private set; }

    public Event<IBikeReserved> BikeReserved { get; private set; }

    public Event<IBikeValidated> BikeValidated { get; private set; }

    public Event<IBikeUnlocked> BikeUnlocked { get; private set; }

    public Event<IBikeValidationFailed> BikeValidationFailed { get; private set; }

    public Event<IBikeReservationFailed> BikeReservationFailed { get; private set; }

    public Event<IBikeUnlockFailed> BikeUnlockFailed { get; private set; }
}
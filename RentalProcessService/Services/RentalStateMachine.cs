using Automatonymous;
using Automatonymous.Binders;
using Common.Commands;
using Common.Events;
using Common.Models.Dtos;
using RentalProcessorService.Saga;

namespace RentalProcessorService.Services;

public sealed class RentalStateMachine : MassTransitStateMachine<RentalState>
{
    private readonly ILogger<RentalStateMachine> _logger;
    
    public RentalStateMachine(
        ILogger<RentalStateMachine> logger
        )
    {
        _logger = logger;

        InstanceState(x => x.State);
        State(() => Processing);
        ConfigureCorrelationIds();
        Initially(SetRentalSummitedHandler());
        During(Processing, SetBikeValidatedHandler(), SetBikeReservedHandler(), SetBikeValidationFailedHandler(),
            SetBikeReservationFailedHandler(), SetBikeUnlockedHandler(), SetBikeUnlockFailedHandler());
        SetCompletedWhenFinalized();
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => RentalSubmitted, x => x.CorrelateById(c => c.Message.CorrelationId)
            .SelectId(c => c.Message.CorrelationId));
        Event(() => BikeValidated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeValidationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeReserved, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeReservationFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlocked, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlockFailed, x => x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<RentalState, IRentalSubmitted> SetRentalSummitedHandler() =>
        When(RentalSubmitted)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Rental submitted to {c.Data.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IValidateBike>("rabbitmq://192.168.1.199/bike-validate", c))
            .TransitionTo(Processing);

    private EventActivityBinder<RentalState, IBikeValidated> SetBikeValidatedHandler() =>
        When(BikeValidated)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Bike validated to {c.Data.CorrelationId} received"))
            .ThenAsync(c => SendCommand<IReserveBike>("rabbitmq://192.168.1.199/bike-reservation", c));

    private EventActivityBinder<RentalState, IBikeReserved> SetBikeReservedHandler() =>
        When(BikeReserved)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Bike reserved to {c.Data.CorrelationId} received"))
            .Then(c => UpdateRental(c.Data.Rental))
            .ThenAsync(c => SendRentalStartNotificationAsync(c))
            .ThenAsync(c => SendCommand<IUnlockBike>("rabbitmq://192.168.1.199/bike-unlock", c));

    private async Task SendRentalStartNotificationAsync(BehaviorContext<RentalState, IRentalMessage> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Message = "Enjoy your ride",
            Subject = "Rental started",
            Reason = "rental-started",
            RentalId = context.Data.Rental.Id
        };

       var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));
        
       await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    private void UpdateRental(RentalDto rentalDto)
    {
    }

    private EventActivities<RentalState> SetBikeUnlockedHandler() =>
        When(BikeUnlockFailed)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Bike unlock failed to {c.Data.CorrelationId} received"));

    private EventActivities<RentalState> SetBikeUnlockFailedHandler() =>
        When(BikeUnlocked)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Bike unlocked to {c.Data.CorrelationId} received"));

    private EventActivityBinder<RentalState, IBikeValidationFailed> SetBikeValidationFailedHandler() =>
        When(BikeValidationFailed)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Bike validated to {c.Data.CorrelationId} received"));

    private EventActivityBinder<RentalState, IBikeReservationFailed> SetBikeReservationFailedHandler() =>
        When(BikeReservationFailed)
            .Then(c => UpdateSagaState(c.Instance, c.Data.Rental))
            .Then(c => _logger.LogInformation($"Bike reservation failed to {c.Data.CorrelationId} received"));

    private static void UpdateSagaState(RentalState state, RentalDto rental)
    {
        var currentDate = DateTime.UtcNow;
        
        state.Created = currentDate;
        state.Updated = currentDate;
        
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

    public State Processing { get; private set; }

    public Event<IRentalSubmitted> RentalSubmitted { get; private set; }

    public Event<IBikeReserved> BikeReserved { get; private set; }

    public Event<IBikeValidated> BikeValidated { get; private set; }

    public Event<IBikeUnlocked> BikeUnlocked { get; private set; }

    public Event<IBikeValidationFailed> BikeValidationFailed { get; private set; }

    public Event<IBikeReservationFailed> BikeReservationFailed { get; private set; }

    public Event<IBikeUnlockFailed> BikeUnlockFailed { get; private set; }
}
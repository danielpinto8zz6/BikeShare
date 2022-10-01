using System;
using System.Threading.Tasks;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Payment;
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
        ILogger<RentalStateMachine> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        InstanceState(x => x.Status);

        ConfigureCorrelationIds();

        Initially(SetRentalSummitedHandler());
        During(Validating, SetBikeValidatedHandler());
        During(Unlocking, SetBikeUnlockedHandler());
        During(InUse, SetBikeLockedHandler());
        DuringAny(SetRentalFailureHandler());

        SetCompletedWhenFinalized();
    }

    private void ConfigureCorrelationIds()
    {
        Event(() => RentalSubmitted, x => x.CorrelateById(c => c.Message.CorrelationId)
            .SelectId(c => c.Message.CorrelationId));
        Event(() => BikeValidated, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeUnlocked, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => BikeLocked, x => x.CorrelateById(c => c.Message.CorrelationId));
        Event(() => RentalFailure, x => x.CorrelateById(c => c.Message.CorrelationId));
    }

    private EventActivityBinder<RentalState, IRentalSubmitted> SetRentalSummitedHandler() =>
        When(RentalSubmitted)
            .TransitionTo(Validating)
            .SendAsync(new Uri($"queue:{nameof(IValidateBike)}"), BuildCommand<IValidateBike>)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.Submitted))
            .Then(c => _logger.LogInformation($"Rental submitted to {c.CorrelationId} received"));

    private EventActivityBinder<RentalState, IBikeValidated> SetBikeValidatedHandler() =>
        When(BikeValidated)
            .TransitionTo(Unlocking)
            .SendAsync(new Uri($"queue:{nameof(IUnlockBike)}"), BuildCommand<IUnlockBike>)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeValidated))
            .Then(c => _logger.LogInformation($"Bike validated to {c.CorrelationId} received"))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetBikeValidatedNotificationAsync(c)));

    private EventActivityBinder<RentalState, IBikeUnlocked> SetBikeUnlockedHandler() =>
        When(BikeUnlocked)
            .TransitionTo(InUse)
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeUnlocked))
            .Then(c => _logger.LogInformation($"Bike unlock to {c.CorrelationId} received"))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetBikeUnlockedNotification(c)));

    private EventActivityBinder<RentalState, IBikeLocked> SetBikeLockedHandler() =>
        When(BikeLocked)
            .Finalize()
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.BikeLocked))
            .Then(c => _logger.LogInformation($"Bike locked to {c.CorrelationId} received"))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetBikeLockedNotification(c)))
            .SendAsync(new Uri($"queue:{nameof(IPaymentRequested)}"), BuildPaymentRequestCommand);

    private EventActivityBinder<RentalState, IRentalFailure> SetRentalFailureHandler() =>
        When(RentalFailure)
            .Finalize()
            .ThenAsync(c => UpdateSagaState(c.Saga, c.Message.Rental, RentalStatus.RentalFailure))
            .Then(c => _logger.LogInformation($"Rental failure to {c.CorrelationId} received"))
            .PublishAsync(c => c.Init<NotificationDto>(NotificationHelper.GetRentalFailureNotification(c)));

    private Task<SendTuple<T>> BuildCommand<T>(BehaviorContext<RentalState, IRentalMessage> context) where T : class
    {
        return context.Init<T>(new
        {
            CorrelationId = context.CorrelationId ?? Guid.Empty,
            Rental = context.Message.Rental
        });
    }

    private Task<SendTuple<IPaymentRequested>> BuildPaymentRequestCommand(
        BehaviorContext<RentalState, IBikeLocked> context)
    {
        var rentalDto = context.Message.Rental;

        var startDate = rentalDto.StartDate ?? DateTime.UtcNow;
        var endDate = rentalDto.EndDate ?? DateTime.UtcNow;

        var payment = new PaymentDto
        {
            Status = PaymentStatus.Requested,
            Username = rentalDto.Username,
            StartDate = startDate,
            EndDate = endDate,
            RentalId = rentalDto.Id
        };

        return context.Init<IPaymentRequested>(new
        {
            CorrelationId = Guid.NewGuid(),
            Payment = payment
        });
    }

    private async Task UpdateSagaState(RentalState state, RentalDto rental, RentalStatus rentalStatus)
    {
        var currentDate = DateTime.UtcNow;

        rental.Status = rentalStatus;
        state.Created = currentDate;
        state.Updated = currentDate;
        state.Status = (int) rentalStatus;
        state.Rental = rental;

        await UpdateRentalAsync(rental);
    }

    private Task<RentalDto> UpdateRentalAsync(RentalDto rental)
    {
        using var scope = _serviceProvider.CreateScope();
        var rentalService = scope.ServiceProvider.GetRequiredService<IRentalService>();

        return rentalService.UpdateAsync(rental.Id, rental);
    }

    public State Validating { get; private set; }
    public State Unlocking { get; private set; }
    public State InUse { get; private set; }

    public Event<IRentalSubmitted> RentalSubmitted { get; private set; }
    public Event<IBikeValidated> BikeValidated { get; private set; }
    public Event<IBikeUnlocked> BikeUnlocked { get; private set; }
    public Event<IBikeLocked> BikeLocked { get; private set; }
    public Event<IRentalFailure> RentalFailure { get; private set; }
}
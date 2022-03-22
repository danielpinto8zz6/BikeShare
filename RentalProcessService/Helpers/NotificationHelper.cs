using Common.Models.Dtos;
using Common.Models.Events;
using MassTransit;
using RentalProcessService.Saga;

namespace RentalProcessService.Helpers;

public static class NotificationHelper
{
    public static async Task SendBikeReservedNotificationAsync(BehaviorContext<RentalState, IRentalMessage> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Body = "Bike reserved",
            Title = "Bike reserved",
            Reason = "bike-reserved",
            RentalId = context.Data.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeUnlockedNotificationAsync(BehaviorContext<RentalState, IBikeUnlocked> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Body = "Bike unlocked",
            Title = "Bike unlocked",
            Reason = "bike-unlocked",
            RentalId = context.Data.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
    
    public static async Task SendBikeValidatedNotificationAsync(BehaviorContext<RentalState, IBikeValidated> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Body = "Bike validated",
            Title = "Bike validated",
            Reason = "bike-validated",
            RentalId = context.Data.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeUnlockFailedNotificationAsync(BehaviorContext<RentalState, IBikeUnlockFailed> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Body = "Bike unlock failed",
            Title = "Bike unlock failed",
            Reason = "bike-unlock-failed",
            RentalId = context.Data.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeValidationFailedNotificationAsync(
        BehaviorContext<RentalState, IBikeValidationFailed> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Body = "Bike validation failed",
            Title = "Bike validation failed",
            Reason = "bike-validation-failed",
            RentalId = context.Data.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeReservationFailedNotificationAsync(
        BehaviorContext<RentalState, IBikeReservationFailed> context)
    {
        var notificationDto = new RentalStartedNotificationDto
        {
            Username = context.Data.Rental.Username,
            Type = nameof(RentalDto),
            Body = "Bike reservation failed",
            Title = "Bike reservation failed",
            Reason = "bike-reservation-failed",
            RentalId = context.Data.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
}
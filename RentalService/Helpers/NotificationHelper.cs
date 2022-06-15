using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Models.Events.Rental;
using MassTransit;
using RentalService.Saga;

namespace RentalService.Helpers;

public static class NotificationHelper
{
    private const string Event = "event";

    private const string RentalId = "rentalId";

    public static async Task SendBikeReservedNotificationAsync(BehaviorContext<RentalState, IRentalMessage> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike reserved",
            Title = "Bike reserved",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-reserved"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeUnlockedNotificationAsync(BehaviorContext<RentalState, IBikeUnlocked> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike unlocked",
            Title = "Bike unlocked",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-unlocked"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
    
    public static async Task SendBikeAttachedNotificationAsync(BehaviorContext<RentalState, IBikeAttached> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike attached",
            Title = "Bike attached",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-attached"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeValidatedNotificationAsync(BehaviorContext<RentalState, IBikeValidated> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike validated",
            Title = "Bike validated",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-validated"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeUnlockFailedNotificationAsync(
        BehaviorContext<RentalState, IBikeUnlockFailed> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike unlock failed",
            Title = "Bike unlock failed",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-unlock-failed"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
    
    public static async Task SendBikeLockFailedNotificationAsync(
        BehaviorContext<RentalState, IBikeLockFailed> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike lock failed",
            Title = "Bike lock failed",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-lock-failed"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeValidationFailedNotificationAsync(
        BehaviorContext<RentalState, IBikeValidationFailed> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike validation failed",
            Title = "Bike validation failed",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-validation-failed"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }

    public static async Task SendBikeReservationFailedNotificationAsync(
        BehaviorContext<RentalState, IBikeReservationFailed> context)
    {
        var notificationDto = new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "Bike reservation failed",
            Title = "Bike reservation failed",
            Data = new Dictionary<string, string>
            {
                {Event, "bike-reservation-failed"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };

        var sendEndpoint = await context.GetSendEndpoint(new Uri("rabbitmq://192.168.1.199/notification"));

        await sendEndpoint.Send<NotificationDto>(notificationDto);
    }
}
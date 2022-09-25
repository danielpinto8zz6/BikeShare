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

    public static RentalNotificationDto GetBikeReservedNotification(BehaviorContext<RentalState, IRentalMessage> context)
    {
        return new RentalNotificationDto
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
    }

    public static RentalNotificationDto GetBikeUnlockedNotification(BehaviorContext<RentalState, IBikeUnlocked> context)
    {
        return new RentalNotificationDto
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
    }
    
    public static RentalNotificationDto GetBikeAttachedNotification(BehaviorContext<RentalState, IBikeAttached> context)
    {
        return new RentalNotificationDto
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
    }

    public static NotificationDto GetBikeValidatedNotificationAsync(BehaviorContext<RentalState, IBikeValidated> context)
    {
        return new RentalNotificationDto
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
    }

    public static RentalNotificationDto GetRentalFailureNotification(
        BehaviorContext<RentalState, IRentalFailure> context)
    {
        return new RentalNotificationDto
        {
            Username = context.Message.Rental.Username,
            Body = "There was an error processing your rental, please try again later.",
            Title = "Rental failed",
            Data = new Dictionary<string, string>
            {
                {Event, "rental-failed"},
                {RentalId, context.Message.Rental.Id.ToString()}
            },
            RentalId = context.Message.Rental.Id
        };
    }
}
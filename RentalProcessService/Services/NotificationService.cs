using Common;
using Common.Models.Dtos;

namespace RentalProcessorService.Services;

public class NotificationService : INotificationService
{
    private readonly IProducer<NotificationDto> _notificationProducer;

    public NotificationService(IProducer<NotificationDto> notificationProducer)
    {
        _notificationProducer = notificationProducer;
    }

    public Task SendRentalStartedNotification(RentalDto rentalDto)
    {
        var rentalStartedNotificationDto = new RentalStartedNotificationDto
        {
            Username = rentalDto.Username,
            Type = nameof(RentalDto),
            Subject = "Enjoy your ride",
            RentalId = rentalDto.Id
        };

        return _notificationProducer.ProduceAsync(rentalStartedNotificationDto);
    }
}
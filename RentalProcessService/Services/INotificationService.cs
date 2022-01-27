using Common.Models.Dtos;

namespace RentalProcessorService.Services;

public interface INotificationService
{
    Task SendRentalStartedNotification(RentalDto rentalDto);
}
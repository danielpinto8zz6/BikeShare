using System.Threading.Tasks;
using Common.Models.Dtos;

namespace Common.Services;

public class NotificationService : INotificationService
{
    private readonly IProducer<NotificationDto> _notificationProducer;

    public NotificationService(IProducer<NotificationDto> notificationProducer)
    {
        _notificationProducer = notificationProducer;
    }

    public Task SendNotificationAsync(NotificationDto notificationDto)
    {
        return _notificationProducer.ProduceAsync(notificationDto);
    }
}
using System.Threading.Tasks;
using Common.Models.Dtos;

namespace NotificationService.Services;

public interface INotificationService
{
    public Task SendNotificationAsync(NotificationDto notificationDto);
}
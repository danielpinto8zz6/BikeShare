using System.Threading.Tasks;
using Common.Models.Dtos;

namespace Common.Services;

public interface INotificationService
{
    Task SendNotificationAsync(NotificationDto notificationDto);
}    
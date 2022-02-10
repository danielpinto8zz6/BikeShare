using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationService.Services;

namespace NotificationService.Consumers;

public class NotificationConsumer : IConsumer<NotificationDto>
{
    private readonly ILogger<NotificationConsumer> _logger;

    private readonly INotificationService _notificationService;

    public NotificationConsumer(
        ILogger<NotificationConsumer> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public Task Consume(ConsumeContext<NotificationDto> context)
    {
        _logger.LogInformation(JsonConvert.SerializeObject(context.Message));

        return _notificationService.SendNotificationAsync(context.Message);
    }
}
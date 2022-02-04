using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Obvs.Logging;

namespace NotificationService.Consumers;

public class NotificationConsumer : IConsumer<NotificationDto>
{
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(ILogger<NotificationConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NotificationDto> context)
    {
        _logger.LogInformation(JsonConvert.SerializeObject(context.Message));
        
        return Task.CompletedTask;
    }
}
using System.Threading.Tasks;
using Common.Models.Dtos;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using NotificationService.Exception;
using NotificationService.Gateways;

namespace NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly ITokenGateway _tokenGateway;

    private readonly IMobileMessagingClient _mobileMessagingClient;

    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ITokenGateway tokenGateway,
        IMobileMessagingClient mobileMessagingClient,
        ILogger<NotificationService> logger)
    {
        _tokenGateway = tokenGateway;
        _mobileMessagingClient = mobileMessagingClient;
        _logger = logger;
    }

    public async Task SendNotificationAsync(NotificationDto notificationDto)
    {
        _logger.LogInformation($"Getting token for {notificationDto.Username}");
        var token = await _tokenGateway.GetTokenByKeyAsync(notificationDto.Username);
        _logger.LogInformation($"Token is: {token}");
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UserTokenNotFoundException(notificationDto.Username);
        }

        var message = new Message
        {
            Token = token,
            Notification = string.IsNullOrWhiteSpace(notificationDto.Title)
                ? null
                : new Notification
                {
                    Body = notificationDto.Body,
                    Title = notificationDto.Title,
                    ImageUrl = notificationDto.ImageUrl
                },
            Data = notificationDto.Data
        };

        var response = await _mobileMessagingClient.Instance.SendAsync(message).ConfigureAwait(false);

        _logger.LogInformation("Successfully sent message: " + response);
    }
}
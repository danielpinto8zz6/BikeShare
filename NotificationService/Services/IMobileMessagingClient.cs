using FirebaseAdmin.Messaging;

namespace NotificationService.Services;

public interface IMobileMessagingClient
{
    public FirebaseMessaging Instance { get; set; }
}
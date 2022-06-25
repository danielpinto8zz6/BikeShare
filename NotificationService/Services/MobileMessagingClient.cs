using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace NotificationService.Services;

public class MobileMessagingClient : IMobileMessagingClient
{
    public FirebaseMessaging Instance { get; set; }

    public MobileMessagingClient()
    {
        var app = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.GetApplicationDefault(),
        });

        Instance = FirebaseMessaging.GetMessaging(app);
    }
}
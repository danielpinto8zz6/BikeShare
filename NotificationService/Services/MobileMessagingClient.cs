using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace NotificationService.Services;

public class MobileMessagingClient : IMobileMessagingClient
{
    public FirebaseMessaging Instance { get; set; }

    public MobileMessagingClient()
    {
        var app = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("bikeshare-6b7f0-firebase-adminsdk-7ofm0-fd08b8cbba.json")
        });

        Instance = FirebaseMessaging.GetMessaging(app);
    }
}
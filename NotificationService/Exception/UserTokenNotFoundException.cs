namespace NotificationService.Exception;

public class UserTokenNotFoundException : System.Exception
{
    public UserTokenNotFoundException(string username) : base(
        $"No token found to user: {username}, ignoring notification.")
    {
    }
}
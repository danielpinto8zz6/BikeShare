namespace Common.Models.Dtos
{
    public class NotificationDto
    {
        public string Username { get; set; }

        public string Body { get; set; }

        public string Title { get; set; }

        public string Reason { get; set; }
        
        public string Type { get; set; }
    }

    public class RentalStartedNotificationDto : NotificationDto

    {
        public Guid RentalId { get; set; }
    }
}
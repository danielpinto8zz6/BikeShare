using System;

namespace Common.Models.Dtos
{
    public class NotificationDto
    {
        public string Username { get; set; }

        public string Message { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }
    }

    public class RentalStartedNotificationDto : NotificationDto

    {
        public Guid RentalId { get; set; }
    }
}
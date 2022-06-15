namespace Common.Models.Dtos
{
    public class NotificationDto
    {
        public string Username { get; set; }

        public string Body { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public IReadOnlyDictionary<string, string> Data { get; set; }
    }

    public class RentalNotificationDto : NotificationDto
    {
        public Guid RentalId { get; set; }
    }
    
    public class PaymentNotificationDto : NotificationDto
    {
        public Guid PaymentId { get; set; }
    }
}
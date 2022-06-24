using Common.Models.Enums;

namespace Common.Models.Dtos
{
    public class PaymentDto : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string Username { get; set; }
        
        public PaymentStatus Status { get; set; }
       
        public double? Duration { get; set; }

        public double? Value { get; set; }

        public Guid RentalId { get; set; }
    }
}
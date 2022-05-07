using Common.Models.Enums;

namespace Common.Models.Dtos
{
    public class PaymentDto : IBaseEntity
    {
        public Guid Id { get; set; }

        public PaymentStatus Status { get; set; }

        public double? Value { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
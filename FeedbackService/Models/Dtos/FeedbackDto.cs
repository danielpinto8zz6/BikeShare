using System;
using Common;

namespace FeedbackService.Models.Dtos
{
    public class FeedbackDto : IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid RentalId { get; set; }

        public string Message { get; set; }

        public int Rating { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
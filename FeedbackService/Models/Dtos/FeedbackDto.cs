using System;
using Common.Models.Dtos;

namespace FeedbackService.Models.Dtos
{
    public class FeedbackDto : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid RentalId { get; set; }

        public string Message { get; set; }

        public int Rating { get; set; }

        public Guid UserId { get; set; }
    }
}
using System;
using Common.Models.Dtos;

namespace FeedbackService.Models.Entities
{
    public class Feedback : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid RentalId { get; set; }

        public string Message { get; set; }

        public int Rating { get; set; }

        public string Username { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }
    }
}   
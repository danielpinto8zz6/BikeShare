using System;

namespace Common.Models.Entities
{
    public class TravelEvent : IBaseEntity
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Guid RentalId { get; set; }

        public Guid UserId { get; set; }
    }
}
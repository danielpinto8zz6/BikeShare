using System;
using Common.Models.Dtos;
using Common.Models.Enums;

namespace RentalService.Models.Entities
{
    public class Rental : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid OriginDockId { get; set; }
        
        public Guid? DestinationDockId { get; set; }
        
        public Guid BikeId { get; set; }
        
        public string Username { get; set; }
        
        public RentalStatus Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }
    }
}
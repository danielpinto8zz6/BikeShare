using System;
using Common;
using LSG.GenericCrud.Models;

namespace RentalService.Models.Entities
{
    public class Rental : IEntity<Guid>, IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid BikeId { get; set; }

        public Guid UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime ModifiedDate { get; set; }
        
        public DateTime DeletedDate { get; set; }
    }
}
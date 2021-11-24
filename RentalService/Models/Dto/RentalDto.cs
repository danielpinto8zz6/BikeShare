using System;
using LSG.GenericCrud.Models;

namespace Common.Models.Dtos
{
    public class RentalDto : IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid BikeId { get; set; }

        public Guid UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
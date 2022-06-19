using System;
using Common.Models.Dtos;
using Common.Models.Enums;

namespace BikeService.Models.Entities
{
    public class Bike : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        
        public string Brand { get; set; }

        public string Model { get; set; }
        
        public BikeType Type { get; set; }

        public BikeStats? Stats { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
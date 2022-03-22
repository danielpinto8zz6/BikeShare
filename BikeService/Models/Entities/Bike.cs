using System;
using Common.Models.Dtos;

namespace BikeService.Models.Entities
{
    public class Bike : IBaseEntity
    {
        public Guid Id { get; set; }
        
        public string Key { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public BikeStats Stats { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
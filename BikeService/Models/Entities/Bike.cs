using System;
using Common;
using LSG.GenericCrud.Models;

namespace BikeService.Models.Entities
{
    public class Bike : IEntity<Guid>, IBaseEntity
    {
        public Guid Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime DeletedDate { get; set; }
    }
}
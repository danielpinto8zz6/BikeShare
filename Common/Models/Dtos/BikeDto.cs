using System;
using Common.Enums;
using LSG.GenericCrud.Models;

namespace Common.Models.Dtos
{
    public class BikeDto : IEntity
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public BikeStatsDto Stats { get; set; }
        
        public BikeStatus Status { get; set; }

        public CoordinatesDto Coordinates { get; set; }
    }
}
using System;
using AutoMapper;
using Common;
using Common.Enums;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BikeService.Models.Entities
{
    public class Bike : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        // db.Bike.createIndex( { Coordinates : "2dsphere" } )
        public GeoJson2DGeographicCoordinates Coordinates { get; set; }
        
        public BikeStatus Status { get; set; }

        [IgnoreMap]
        public DateTime CreatedDate { get; set; }

        [IgnoreMap]
        public DateTime? ModifiedDate { get; set; }

        [IgnoreMap]
        public DateTime? DeletedDate { get; set; }
    }
}
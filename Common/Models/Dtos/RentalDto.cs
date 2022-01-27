using System;
using Common.Enums;
using LSG.GenericCrud.Models;

namespace Common.Models.Dtos
{
    public class RentalDto : IEntity
    {
        public Guid Id { get; set; }

        public Guid BikeId { get; set; }

        public string BikeKey { get; set; }

        public string Username { get; set; }

        public RentalStatus Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
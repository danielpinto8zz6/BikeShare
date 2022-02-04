using System;

namespace Common.Models.Dtos
{
    public class BikeUnlockDto
    {
        public Guid BikeId { get; set; }

        public string BikeKey { get; set; }

        public string Username { get; set; }

        public Guid RentalId { get; set; }
    }
}
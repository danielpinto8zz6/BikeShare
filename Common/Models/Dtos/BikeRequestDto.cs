using System;

namespace Common.Models.Dtos
{
    public class BikeRequestDto
    {
        public Guid BikeId { get; set; }

        public string BikeKey { get; set; }

        public string Username { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
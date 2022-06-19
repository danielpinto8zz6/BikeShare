using Common.Models.Enums;

namespace Common.Models.Dtos
{
    public class BikeDto : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public BikeType Type { get; set; }

        public BikeStatsDto? Stats { get; set; }
    }
}
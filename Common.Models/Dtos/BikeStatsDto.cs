using LSG.GenericCrud.Models;

namespace Common.Models.Dtos
{
    public class BikeStatsDto : IEntity
    {
        public Guid Id { get; set; }

        public int ChargeLevel { get; set; }

        public int ServiceMinutes { get; set; }
    }
}
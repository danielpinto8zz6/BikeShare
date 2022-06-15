namespace Common.Models.Dtos
{
    public class BikeStatsDto : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public int ChargeLevel { get; set; }

        public int ServiceMinutes { get; set; }
    }
}
namespace Common.Models.Dtos
{
    public class BikeDto : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public BikeStatsDto Stats { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
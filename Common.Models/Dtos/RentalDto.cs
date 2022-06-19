using Common.Models.Enums;

namespace Common.Models.Dtos
{
    public class RentalDto : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid? OriginDockId { get; set; }
        
        public Guid? DestinationDockId { get; set; }

        public Guid BikeId { get; set; }
        
        public string Username { get; set; }

        public RentalStatus Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
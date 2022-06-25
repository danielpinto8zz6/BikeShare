using Common.Models.Dtos;

namespace Common.TravelEvent.Entities
{
    public class TravelEvent : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? DeletedDate { get; set; }

        public CoordinatesDto Coordinates { get; set; }

        public Guid RentalId { get; set; }

        public string Username { get; set; }
    }
}
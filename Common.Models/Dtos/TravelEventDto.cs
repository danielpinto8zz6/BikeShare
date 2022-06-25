namespace Common.Models.Dtos;

public class TravelEventDto : IEntity<Guid>
{
    public Guid Id { get; set; }

    public CoordinatesDto Coordinates { get; set; }

    public Guid RentalId { get; set; }

    public string Username { get; set; }
}
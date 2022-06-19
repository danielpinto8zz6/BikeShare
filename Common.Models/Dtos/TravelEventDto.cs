namespace Common.Models.Dtos;

public class TravelEventDto : IEntity<Guid>
{
    public Guid Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public Guid RentalId { get; set; }

    public Guid UserId { get; set; }
}
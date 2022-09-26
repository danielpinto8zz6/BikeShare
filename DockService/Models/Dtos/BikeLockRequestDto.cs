namespace DockService.Models.Dtos;

public class BikeLockRequestDto
{
    public Guid BikeId { get; set; }

    public Guid DockId { get; set; }
}
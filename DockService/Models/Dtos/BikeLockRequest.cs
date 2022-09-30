namespace DockService.Models.Dtos;

public class BikeLockRequest
{
    public Guid BikeId { get; set; }

    public Guid DockId { get; set; }
}
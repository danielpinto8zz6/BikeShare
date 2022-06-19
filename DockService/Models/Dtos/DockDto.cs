using Common.Models.Dtos;

namespace DockService.Models.Dtos;

public class DockDto : IEntity<Guid>
{
    public Guid Id { get; set; }
    
    public Guid? BikeId { get; set; }

    public CoordinatesDto? Coordinates { get; set; }

    public string? Address { get; set; }
}
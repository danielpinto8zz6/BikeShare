namespace DockInternalServiceEmulator.Models.Dtos;

public class BikeAttachRequestDto
{
    public Guid BikeId { get; set; }

    public Guid DockId { get; set; }
}
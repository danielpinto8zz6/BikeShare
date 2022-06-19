using Common.Models.Dtos;
using MongoDB.Driver.GeoJsonObjectModel;

namespace DockService.Models.Entities;

public class Dock : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
    
    public Guid? BikeId { get; set; }

    public GeoJson2DGeographicCoordinates? Coordinates { get; set; }

    public string? Address { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }
}
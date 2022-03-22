using Common.Models.Dtos;

namespace DockService.Models.Dtos;

public class DockDto : IBaseEntity
{
    public Guid Id { get; set; }
    
    public Guid? BikeId { get; set; }

    public CoordinatesDto Coordinates { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public DateTime? DeletedDate { get; set; }  
}
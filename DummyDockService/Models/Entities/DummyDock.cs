using Common;
using Common.Models.Dtos;

namespace DummyDockService.Models.Entities;

public class DummyDock : IBaseEntity
{
    public Guid Id { get; set; }
    
    public Guid BikeId { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
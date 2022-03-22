using LSG.GenericCrud.Models;

namespace DummyDockService.Models.Dtos;

public class DummyDockDto : IEntity
{
    public Guid Id { get; set; }
    
    public Guid? BikeId { get; set; }
}
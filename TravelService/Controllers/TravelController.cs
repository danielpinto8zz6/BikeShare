using Common.Models.Dtos;
using Common.Services;
using Common.TravelEvent.Services;
using Microsoft.AspNetCore.Mvc;

namespace TravelService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TravelController : ControllerBase
{
    private readonly IProducer<TravelEventDto> _producer;

    private readonly ITravelEventService _service;

    public TravelController(
        IProducer<TravelEventDto> producer, 
        ITravelEventService service)
    {
        _producer = producer;
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<TravelEventDto>> Create(
        [FromHeader(Name = "UserId")] string userId,
        [FromBody] TravelEventDto travelEvent)
    {
        travelEvent.Username = userId;
        
        await _producer.ProduceAsync(travelEvent);
        
        return Accepted();
    }
    
    [HttpGet("rental/{rentalId}")]
    public async Task<ActionResult<IEnumerable<TravelEventDto>>> Create(Guid rentalId)
    {
        var result = await _service.GetByRentalIdAsync(rentalId);
        return Ok(result);
    }
}
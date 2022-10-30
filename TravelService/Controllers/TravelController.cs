using Common.Models.Dtos;
using Common.Services;
using Common.TravelEvent.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace TravelService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TravelController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ITravelEventService _service;

    public TravelController(
        ITravelEventService service, 
        IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<ActionResult<TravelEventDto>> Create(
        [FromHeader(Name = "UserId")] string userId,
        [FromBody] TravelEventDto travelEvent)
    {
        travelEvent.Username = userId;
        
        await _publishEndpoint.Publish(travelEvent);
        
        return Accepted();
    }
    
    [HttpGet("rental/{rentalId}")]
    public async Task<ActionResult<IEnumerable<TravelEventDto>>> Create(Guid rentalId)
    {
        var result = await _service.GetByRentalIdAsync(rentalId);
        return Ok(result);
    }
}
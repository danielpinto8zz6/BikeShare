using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace TravelService.Controllers;

[ApiController]
[Route("[controller]")]
public class TravelController : ControllerBase
{
    private readonly IProducer<TravelEventDto> _producer;

    public TravelController(
        IProducer<TravelEventDto> producer)
    {
        _producer = producer;
    }

    [HttpPost]
    public async Task<ActionResult<TravelEventDto>> Create([FromBody] TravelEventDto travelEvent)
    {
        await _producer.ProduceAsync(travelEvent);
        return Accepted();
    }
}
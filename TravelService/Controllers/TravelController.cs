using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Services;
using LSG.GenericCrud.Controllers;
using LSG.GenericCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace TravelService.Controllers;

[ApiController]
[Route("[controller]")]
public class TravelController : CrudControllerBase<Guid, TravelEventDto>
{
    private readonly IProducer<TravelEventDto> _producer;

    public TravelController(
        ICrudService<Guid, TravelEventDto> service,
        IProducer<TravelEventDto> producer)
        : base(service)
    {
        _producer = producer;
    }

    [HttpPost]
    public override async Task<ActionResult<TravelEventDto>> Create([FromBody] TravelEventDto travelEvent)
    {
        await _producer.ProduceAsync(travelEvent);
        return CreatedAtAction("GetById", new
        {
            id = travelEvent.Id
        }, travelEvent);
    }
}
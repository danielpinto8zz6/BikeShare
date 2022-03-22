using System.Threading.Tasks;
using Common.Models.Dtos;
using Common.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TravelEventService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TravelEventController : ControllerBase
    {
        private readonly IProducer<TravelEventDto> _producer;

        private readonly ILogger<TravelEventController> _logger;

        public TravelEventController(ILogger<TravelEventController> logger, IBus bus, IProducer<TravelEventDto> producer)
        {
            _logger = logger;
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> AddWeatherForecast(TravelEventDto travelEventDto)
        {
            if (travelEventDto != null)
            {
                await _producer.ProduceAsync(travelEventDto);
                
                return Ok(travelEventDto);
            }

            return BadRequest();
        }
    }
}
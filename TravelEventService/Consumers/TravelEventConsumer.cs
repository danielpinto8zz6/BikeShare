using Common.Models.Dtos;
using Common.TravelEvent.Services;
using MassTransit;

namespace TravelEventService.Consumers
{
    public class TravelEventConsumer : IConsumer<TravelEventDto>
    {
        private readonly ITravelEventService _travelEventService;

        public TravelEventConsumer(ITravelEventService travelEventService)
        {
            _travelEventService = travelEventService;
        }

        public async Task Consume(ConsumeContext<TravelEventDto> context)
        {
            await _travelEventService.CreateAsync(context.Message);
        }
    }
}
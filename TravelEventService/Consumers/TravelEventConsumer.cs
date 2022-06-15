using LSG.GenericCrud.Services;
using MassTransit;
using MongoDB.Bson.IO;
using TravelEventService.Entities;

namespace TravelEventService.Consumers
{
    public class TravelEventConsumer : IConsumer<TravelEvent>
    {
        private readonly ICrudService<Guid, TravelEvent> _crudService;

        public TravelEventConsumer(ICrudService<Guid, TravelEvent> crudService)
        {
            _crudService = crudService;
        }

        public async Task Consume(ConsumeContext<TravelEvent> context)
        {
            await _crudService.CreateAsync(context.Message);
        }
    }
}
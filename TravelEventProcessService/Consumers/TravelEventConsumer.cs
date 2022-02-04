using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using LSG.GenericCrud.Services;
using MassTransit;
using Newtonsoft.Json;

namespace TravelEventProcessService.Consumers
{
    public class TravelEventConsumer : IConsumer<TravelEventDto>
    {
        private readonly ICrudService<Guid, TravelEventDto> _crudService;

        public TravelEventConsumer(ICrudService<Guid, TravelEventDto> crudService)
        {
            _crudService = crudService;
        }

        public async Task Consume(ConsumeContext<TravelEventDto> context)
        {
            Console.WriteLine("Message received:");
            Console.WriteLine(JsonConvert.SerializeObject(context.Message));

            await _crudService.CreateAsync(context.Message);
        }
    }
}
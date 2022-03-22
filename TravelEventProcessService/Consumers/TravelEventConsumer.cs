using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using LSG.GenericCrud.Services;
using MassTransit;
using Newtonsoft.Json;
using TravelEventProcessService.Entities;

namespace TravelEventProcessService.Consumers
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
            Console.WriteLine("Message received:");
            Console.WriteLine(JsonConvert.SerializeObject(context.Message));

            await _crudService.CreateAsync(context.Message);
        }
    }
}
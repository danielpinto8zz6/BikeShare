using System;
using System.Threading.Tasks;
using Common.Models.Dtos;
using MassTransit;
using Newtonsoft.Json;

namespace TravelEventProcessService.Consumers
{
    public class TravelEventConsumer : IConsumer<TravelEventDto>
    {
        readonly IPublishEndpoint _publishEndpoint;
        public async Task Consume(ConsumeContext<TravelEventDto> context)
        {
            Console.WriteLine("Message received:");
            Console.WriteLine(JsonConvert.SerializeObject(context.Message));
        }
    }
}
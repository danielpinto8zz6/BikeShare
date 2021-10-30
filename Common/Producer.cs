using System;
using System.Threading.Tasks;
using Common.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Common
{
    public class Producer<T> : IProducer<T>
    {
        private readonly IBus _bus;

        private readonly RabbitMqConfiguration _rabbitMqConfiguration;

        public Producer(IBus bus, IOptions<RabbitMqConfiguration> rabbitMqConfiguration)
        {
            _bus = bus;
            _rabbitMqConfiguration = rabbitMqConfiguration.Value;
        }

        public async Task ProduceAsync(T value, string queue)
        {
            var uri = new Uri($"{_rabbitMqConfiguration.Host}/{queue}");
            
            var endPoint = await _bus.GetSendEndpoint(uri);

            await endPoint.Send(value);
        }
    }
}
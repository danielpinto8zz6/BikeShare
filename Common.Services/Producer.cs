using MassTransit;

namespace Common.Services
{
    public class Producer<T> : IProducer<T>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        
        public Producer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task ProduceAsync(T value)
        {
            return value != null ? _publishEndpoint.Publish(value) : Task.CompletedTask;
        }
    }
}
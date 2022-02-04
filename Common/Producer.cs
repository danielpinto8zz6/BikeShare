using System.Threading.Tasks;
using MassTransit;

namespace Common
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
            return _publishEndpoint.Publish(value);
        }
    }
}
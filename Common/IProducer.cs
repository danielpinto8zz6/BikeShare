using System.Threading.Tasks;

namespace Common
{
    public interface IProducer<T>
    {
        Task ProduceAsync(T message, string queue);
    }
}
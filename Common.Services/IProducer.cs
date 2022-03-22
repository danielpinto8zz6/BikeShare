namespace Common.Services
{
    public interface IProducer<T>
    {
        Task ProduceAsync(T message);
    }
}
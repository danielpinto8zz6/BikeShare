namespace DockService.Services;

public interface IMqttPublisher
{
    Task PublishAsync<T>(string topic, T payload);
}
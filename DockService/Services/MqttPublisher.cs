using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DockService.Services;

public class MqttPublisher : IMqttPublisher
{
    private readonly MqttFactory _mqttFactory = new();
    private readonly MqttClientOptions _mqttClientOptions;
    
    public MqttPublisher(IOptions<MqttConfiguration> mqttOptions)
    {
        var mqttConfiguration = mqttOptions.Value;
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttConfiguration.Server, mqttConfiguration.Port)
            .WithClientId(mqttConfiguration.ClientId)
            .Build();
    }

    public async Task PublishAsync<T>(string topic, T payload)
    {
        using var mqttClient = _mqttFactory.CreateMqttClient();
        
        await mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);

        var payloadStr = JsonConvert.SerializeObject(payload);
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payloadStr)
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
    }
}
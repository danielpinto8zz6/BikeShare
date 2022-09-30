using System.Text;
using DockService.Models.Dtos;
using DockService.Services;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DockService;

public class MqttSubscriber : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MqttClientOptions _mqttClientOptions;
    private const string Topic = "bike-attached";
    
    public MqttSubscriber(IOptions<MqttConfiguration> mqttOptions, IServiceProvider serviceProvider)
    {
        var mqttConfiguration = mqttOptions.Value;
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttConfiguration.Server, mqttConfiguration.Port)
            .WithClientId(mqttConfiguration.ClientId)
            .Build();
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();

        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var bikeAttached = JsonConvert.DeserializeObject<BikeLockRequest>(message);

            using var scope = _serviceProvider.CreateScope();
            var dockManagerService = scope.ServiceProvider.GetRequiredService<IDockManagerService>();
            await dockManagerService.LockBikeAsync(bikeAttached);
        };

        await mqttClient.ConnectAsync(_mqttClientOptions, stoppingToken);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic("bike-attached"); })
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);
    }
}
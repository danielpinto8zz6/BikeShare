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
    private readonly IMqttClient _mqttClient;
    private readonly MqttFactory _mqttFactory = new();
    private readonly ILogger<MqttSubscriber> _logger;

    public MqttSubscriber(
        IOptions<MqttConfiguration> mqttOptions, 
        IServiceProvider serviceProvider, 
        ILogger<MqttSubscriber> logger)
    {
        var mqttConfiguration = mqttOptions.Value;
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttConfiguration.Server, mqttConfiguration.Port)
            .WithClientId(mqttConfiguration.ClientId + Guid.NewGuid())
            //.WithCredentials(mqttConfiguration.Username, mqttConfiguration.Password)
            .Build();
        _serviceProvider = serviceProvider;
        _logger = logger;
        _mqttClient = _mqttFactory.CreateMqttClient();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var bikeAttached = JsonConvert.DeserializeObject<BikeLockRequest>(message);

            _logger.LogInformation(
                $"Received bike attached event for bike {bikeAttached.BikeId} on dock {bikeAttached.DockId}");
                
            using var scope = _serviceProvider.CreateScope();
            var dockManagerService = scope.ServiceProvider.GetRequiredService<IDockManagerService>();
            
            return dockManagerService.LockBikeAsync(bikeAttached);
        };

        await _mqttClient.ConnectAsync(_mqttClientOptions, stoppingToken);

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => { f.WithTopic(Topic); })
            .Build();

        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);
    }

    public override void Dispose()
    {
        _mqttClient.DisconnectAsync();
        _mqttClient.Dispose();
    }
}
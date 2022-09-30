using System.Text;
using DockService.Models.Dtos;
using DockService.Services;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DockService;

public class MqttService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    
    public MqttService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttFactory();
        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("192.168.1.199", 31883)
                .Build();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var bikeAttached = JsonConvert.DeserializeObject<BikeLockRequest>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dockManagerService = scope.ServiceProvider.GetRequiredService<IDockManagerService>();
                    dockManagerService.LockBikeAsync(bikeAttached);
                }
                
                return Task.CompletedTask;
            };

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic("bike-attached"); })
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
using System.Text.Json;
using Common.Models.Commands.Rental;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using dotnet_etcd.interfaces;
using MassTransit;

namespace DockInternalServiceEmulator.Consumers;

public class UnlockBikeConsumer : IConsumer<IUnlockBike>
{
    private readonly IEtcdClient _etcdClient;

    public UnlockBikeConsumer(IEtcdClient etcdClient)
    {
        _etcdClient = etcdClient;
    }

    public async Task Consume(ConsumeContext<IUnlockBike> context)
    {
        var rental = context.Message.Rental;
        
        rental.Status = RentalStatus.BikeUnlocked;
        rental.StartDate = DateTime.UtcNow;

        var rentalMessageStr = JsonSerializer.Serialize<IRentalMessage>(context.Message);
        await _etcdClient.PutAsync(rental.BikeId.ToString(), rentalMessageStr);

        await _etcdClient.DeleteAsync(rental.OriginDockId.ToString());
        
        var endpoint = await context.GetSendEndpoint(new Uri($"queue:{nameof(IBikeUnlocked)}"));
        await endpoint.Send<IBikeUnlocked>(new
        {
            context.CorrelationId,
            context.Message.Rental
        });
        
        //TODO: Lamp on
    }
}
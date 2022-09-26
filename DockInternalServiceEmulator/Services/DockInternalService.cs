using System.Text.Json;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using DockInternalServiceEmulator.Models.Dtos;
using dotnet_etcd.interfaces;
using MassTransit;

namespace DockInternalServiceEmulator.Services;

public class DockInternalService : IDockInternalService
{
    private readonly IBus _bus;

    private readonly IEtcdClient _etcdClient;

    public DockInternalService(
        IBus bus,
        IEtcdClient etcdClient)
    {
        _bus = bus;
        _etcdClient = etcdClient;
    }

    public async Task AttachBikeAsync(BikeAttachRequestDto bikeAttachRequestDto)
    {
        var rentalMessageStr = await _etcdClient.GetValAsync(bikeAttachRequestDto.BikeId.ToString());
        await _etcdClient.DeleteAsync(bikeAttachRequestDto.BikeId.ToString());
        
        var rentalMessage = JsonSerializer.Deserialize<RentalMessage>(rentalMessageStr);

        if (rentalMessage == null)
        {
            throw new Exception();
        }

        rentalMessage.Rental.Status = RentalStatus.BikeAttached;
        rentalMessage.Rental.EndDate = DateTime.UtcNow;
        rentalMessage.Rental.DestinationDockId = bikeAttachRequestDto.DockId;

        var result = await _etcdClient.GetValAsync(bikeAttachRequestDto.DockId.ToString());
        if (string.IsNullOrWhiteSpace(result))
        {
            throw new Exception($"Error, dock with id: {bikeAttachRequestDto.DockId} already contains a bike attached!");
        }
        
        await _etcdClient.PutAsync(bikeAttachRequestDto.DockId.ToString(), bikeAttachRequestDto.BikeId.ToString());
        
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nameof(IBikeAttached)}"));
        await endpoint.Send<IBikeAttached>(rentalMessage);
        
        //TODO: Lamp off
    }
}
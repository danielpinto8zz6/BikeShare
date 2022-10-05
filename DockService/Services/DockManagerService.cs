using System.Text.Json;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using DockService.Models.Dtos;
using dotnet_etcd.interfaces;
using MassTransit;

namespace DockService.Services;

public class DockManagerService : IDockManagerService
{
    private readonly IDockService _dockService;
    private readonly IEtcdClient _etcdClient;
    private readonly IBus _bus;
    private readonly ILogger<DockService> _logger;
    private readonly IMqttPublisher _mqttPublisher;

    public DockManagerService(
        IDockService dockService,
        IEtcdClient etcdClient,
        IBus bus,
        ILogger<DockService> logger, 
        IMqttPublisher mqttPublisher)
    {
        _dockService = dockService;
        _etcdClient = etcdClient;
        _bus = bus;
        _logger = logger;
        _mqttPublisher = mqttPublisher;
    }

    public async Task UnlockBikeAsync(IRentalMessage rentalMessage)
    {
        var rental = rentalMessage.Rental;

        try
        {
            var dockDto = await _dockService.GetByBikeId(rentalMessage.Rental.BikeId);

            await DetachBikeFromDock(dockDto);

            rental.OriginDockId = dockDto.Id;
            rental.Status = RentalStatus.BikeUnlocked;
            rental.StartDate = DateTime.UtcNow;

            await SendBikeUnlocked(rentalMessage);

            var rentalMessageStr = JsonSerializer.Serialize(rentalMessage);
            await _etcdClient.PutAsync(rental.BikeId.ToString(), rentalMessageStr);

            await SendDockStateChangeRequestAsync(dockDto.Id, DockState.Open);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating bike status");

            rental.Status = RentalStatus.RentalFailed;

            await SendBikeUnlockFailed(rentalMessage);
        }
    }

    public async Task LockBikeAsync(BikeLockRequest bikeLockRequest)
    {
        await AttachBikeToDockAsync(bikeLockRequest);

        await SendBikeLockedEventAsync(bikeLockRequest);

        await SendDockStateChangeRequestAsync(bikeLockRequest.DockId, DockState.Closed);
    }

    private async Task AttachBikeToDockAsync(BikeLockRequest bikeLockRequest)
    {
        var dockDto = await _dockService.GetByIdAsync(bikeLockRequest.DockId);
        if (dockDto.BikeId != null)
        {
            throw new InvalidOperationException($"Dock with id: {dockDto.Id} already has a bike attached!");
        }

        dockDto.BikeId = bikeLockRequest.BikeId;
        await _dockService.UpdateAsync(dockDto.Id, dockDto);
    }

    private async Task DetachBikeFromDock(DockDto dockDto)
    {
        dockDto.BikeId = null;

        await _dockService.UpdateAsync(dockDto.Id, dockDto);
    }

    private async Task SendBikeLockedEventAsync(BikeLockRequest bikeLockRequest)
    {
        var rentalMessageStr = await _etcdClient.GetValAsync(bikeLockRequest.BikeId.ToString());
        await _etcdClient.DeleteAsync(bikeLockRequest.BikeId.ToString());
        
        if (string.IsNullOrWhiteSpace(rentalMessageStr))
        {
            throw new NotFoundException(
                $"Rental for bike with identifier {bikeLockRequest.BikeId} not found!");
        }
        
        var rentalMessage = JsonSerializer.Deserialize<RentalMessage>(rentalMessageStr);
        if (rentalMessage == null)
        {
            throw new ArgumentNullException();
        }
        
        rentalMessage.Rental.Status = RentalStatus.BikeLocked;
        rentalMessage.Rental.EndDate = DateTime.UtcNow;
        rentalMessage.Rental.DestinationDockId = bikeLockRequest.DockId;

        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nameof(IBikeLocked)}"));
        await endpoint.Send<IBikeLocked>(rentalMessage);
    }

    private async Task SendBikeUnlockFailed(IRentalMessage rentalMessage)
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nameof(IRentalFailed)}"));
        await endpoint.Send<IRentalFailed>(rentalMessage);
    }

    private async Task SendBikeUnlocked(IRentalMessage rentalMessage)
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nameof(IBikeUnlocked)}"));
        await endpoint.Send<IBikeUnlocked>(rentalMessage);
    }

    private Task SendDockStateChangeRequestAsync(Guid dockId, DockState action)
    {
        const string dockStateChangeTopic = "dock-state-change";
        
        var request =new DockStateChangeRequest
        {
            State = action,
            DockId = dockId
        };

        _logger.LogInformation($"Sending dock state {request.State} for dock {request.DockId}");
        
        return _mqttPublisher.PublishAsync(dockStateChangeTopic, request);
    }
}
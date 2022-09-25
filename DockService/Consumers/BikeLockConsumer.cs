using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using DockService.Models.Dtos;
using DockService.Services;
using MassTransit;
using MongoDB.Bson.IO;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DockService.Consumers;

public class BikeLockConsumer : IConsumer<ILockBike>
{
    private readonly ILogger<ILockBike> _logger;

    private readonly IDockService _dockService;

    public BikeLockConsumer(IDockService dockService, ILogger<ILockBike> logger)
    {
        _dockService = dockService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ILockBike> context)
    {
        _logger.LogInformation($"Lock bike to {context.CorrelationId} was received");
        _logger.LogInformation(JsonConvert.SerializeObject(context.Message.Rental));

        try
        {
            var dockDto =
                await _dockService.GetByIdAsync(context.Message.Rental.DestinationDockId ?? throw new NullReferenceException());

            await AttachBikeToDock(dockDto, context.Message.Rental.BikeId);
            
            UpdateRentalState(context.Message.Rental, RentalStatus.BikeLocked);

            await context.RespondAsync<IBikeLocked>(new
            {
                context.CorrelationId,
                context.Message.Rental
            });
            
            _logger.LogInformation("sent bike attached");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating bike status");

            UpdateRentalState(context.Message.Rental, RentalStatus.RentalFailure);

            await context.RespondAsync<IRentalFailure>(new
            {
                context.CorrelationId,
                context.Message.Rental
            });
        }
    }

    private async Task AttachBikeToDock(DockDto dockDto, Guid bikeId)
    {
        dockDto.BikeId = bikeId;

        await _dockService.UpdateAsync(dockDto.Id, dockDto);
    }

    private static void UpdateRentalState(RentalDto rentalDto, RentalStatus status)
    {
        rentalDto.Status = status;
    }
}
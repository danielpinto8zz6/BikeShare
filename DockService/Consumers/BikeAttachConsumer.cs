using Common.Models.Commands;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events;
using Common.Models.Events.Rental;
using DockService.Models.Dtos;
using DockService.Services;
using MassTransit;

namespace DockService.Consumers;

public class BikeAttachConsumer : IConsumer<IAttachBike>
{
    private readonly ILogger<IAttachBike> _logger;

    private readonly IDockService _dockService;

    public BikeAttachConsumer(IDockService dockService, ILogger<IAttachBike> logger)
    {
        _dockService = dockService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IAttachBike> context)
    {
        _logger.LogInformation($"Attach bike to {context.CorrelationId} was received");

        try
        {
            var dockDto =
                await _dockService.GetByIdAsync(context.Message.Rental.DestinationDockId ?? throw new NullReferenceException());

            await AttachBikeToDock(dockDto, context.Message.Rental.BikeId);

            UpdateRentalState(context.Message.Rental, RentalStatus.BikeAttached);

            await SendBikeAttached(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating bike status");

            UpdateRentalState(context.Message.Rental, RentalStatus.BikeAttachFailed);

            await SendBikeAttachFailed(context);
        }
    }

    private async Task AttachBikeToDock(DockDto dockDto, Guid bikeId)
    {
        dockDto.BikeId = bikeId;

        await _dockService.UpdateAsync(dockDto.Id, dockDto);
    }

    private static async Task SendBikeAttachFailed(ConsumeContext<IAttachBike> context)
    {
        await context.Publish<IBikeAttachFailed>(new
        {
            context.CorrelationId,
            context.Message.Rental
        });
    }

    private static async Task SendBikeAttached(ConsumeContext<IAttachBike> context)
    {
        await context.Publish<IBikeAttached>(new
        {
            context.CorrelationId,
            context.Message.Rental
        });
    }

    private static void UpdateRentalState(RentalDto rentalDto, RentalStatus status)
    {
        rentalDto.Status = status;
    }
}
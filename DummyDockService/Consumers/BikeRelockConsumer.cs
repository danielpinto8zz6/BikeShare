using Common.Models.Commands;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events;
using Common.Models.Events.Rental;
using DummyDockService.Models.Dtos;
using LSG.GenericCrud.Exceptions;
using LSG.GenericCrud.Services;
using MassTransit;

namespace DummyDockService.Consumers;

public class BikeRelockConsumer : IConsumer<IUnlockBike>
{
    private readonly ICrudService<Guid, DummyDockDto?> _service;

    private readonly ILogger<BikeRelockConsumer> _logger;

    public BikeRelockConsumer(
        ICrudService<Guid, DummyDockDto?> service,
        ILogger<BikeRelockConsumer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IUnlockBike> context)
    {
        var rentalDto = context.Message.Rental;

        DummyDockDto? dummyDockDto = null;

        try
        {
            dummyDockDto =
                await _service.GetByIdAsync(rentalDto.DestinationDockId ?? throw new NullReferenceException());
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "Dock with identifier: {rentalDto.DestinationDockId} not found!",
                rentalDto.DestinationDockId);

            dummyDockDto ??= new DummyDockDto
            {
                Id = rentalDto.DestinationDockId ?? throw new NullReferenceException(),
                BikeId = rentalDto.BikeId
            };
        }

        dummyDockDto!.BikeId = rentalDto.BikeId;

        await _service.UpdateAsync(dummyDockDto.Id, dummyDockDto);

        UpdateRentalState(context.Message.Rental, RentalStatus.BikeLocked);

        await context.Publish<IBikeLocked>(new
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
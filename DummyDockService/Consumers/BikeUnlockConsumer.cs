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

public class BikeUnlockConsumer : IConsumer<IUnlockBike>
{
    private readonly ICrudService<Guid, DummyDockDto> _service;

    private readonly ILogger<BikeUnlockConsumer> _logger;

    public BikeUnlockConsumer(
        ICrudService<Guid, DummyDockDto> service,
        ILogger<BikeUnlockConsumer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IUnlockBike> context)
    {
        var rentalDto = context.Message.Rental;

        try
        {
            var dummyDockDto = await _service.GetByIdAsync(rentalDto.OriginDockId ?? throw new NullReferenceException());

            dummyDockDto.BikeId = null;

            await _service.UpdateAsync(dummyDockDto.Id, dummyDockDto);

            UpdateRentalState(context.Message.Rental, RentalStatus.BikeUnlocked);

            await context.Publish<IBikeUnlocked>(new
            {
                context.CorrelationId,
                context.Message.Rental
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dock with identifier: {rentalDto.OriginDockId} not found!", rentalDto.OriginDockId);

            UpdateRentalState(context.Message.Rental, RentalStatus.BikeUnlockFailed);

            await context.Publish<IBikeUnlockFailed>(new
            {
                context.CorrelationId,
                context.Message.Rental
            });
        }
    }

    private static void UpdateRentalState(RentalDto rentalDto, RentalStatus status)
    {
        rentalDto.Status = status;
    }
}
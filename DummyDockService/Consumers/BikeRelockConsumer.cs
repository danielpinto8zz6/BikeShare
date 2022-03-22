using Common.Models.Commands;
using Common.Models.Events;
using DummyDockService.Models.Dtos;
using LSG.GenericCrud.Exceptions;
using LSG.GenericCrud.Services;
using MassTransit;

namespace DummyDockService.Consumers;

public class BikeRelockConsumer : IConsumer<IUnlockBike>
{
    private readonly ICrudService<Guid, DummyDockDto> _service;

    private readonly ILogger<BikeRelockConsumer> _logger;

    public BikeRelockConsumer(
        ICrudService<Guid, DummyDockDto> service, 
        ILogger<BikeRelockConsumer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IUnlockBike> context)
    {
        var rentalDto = context.Message.Rental;
        
        DummyDockDto dummyDockDto = null;

        try
        {
            dummyDockDto = await _service.GetByIdAsync(rentalDto.DockId);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "Dock with identifier: {rentalDto.DockId} not found!", rentalDto.DockId);

            dummyDockDto ??= new DummyDockDto
            {
                Id = rentalDto.DockId,
                BikeId = rentalDto.BikeId
            };
        }

        dummyDockDto.BikeId = rentalDto.BikeId;
        
        await _service.UpdateAsync(dummyDockDto.Id, dummyDockDto);
        
        await context.Publish<IBikeLocked>(new
        {
            context.Message.CorrelationId,
            context.Message.Rental
        });
    }
}
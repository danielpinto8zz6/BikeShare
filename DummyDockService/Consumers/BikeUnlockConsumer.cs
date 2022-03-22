using Common.Models.Commands;
using Common.Models.Events;
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
        
        DummyDockDto dummyDockDto;

        try
        {
            dummyDockDto = await _service.GetByIdAsync(rentalDto.DockId);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "Dock with identifier: {rentalDto.DockId} not found!", rentalDto.DockId);
            
            return;
        }

        dummyDockDto.BikeId = null;
        
        await _service.UpdateAsync(dummyDockDto.Id, dummyDockDto);
        
        await context.Publish<IBikeUnlocked>(new
        {
            context.Message.CorrelationId,
            context.Message.Rental
        });
    }
}
using Common.Models.Commands;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events;
using Common.Models.Events.Rental;
using DockService.Models.Dtos;
using DockService.Services;
using MassTransit;

namespace DockService.Consumers
{
    public class BikeReservationConsumer : IConsumer<IReserveBike>
    {
        private readonly ILogger<BikeReservationConsumer> _logger;

        private readonly IDockService _dockService;

        public BikeReservationConsumer(IDockService dockService, ILogger<BikeReservationConsumer> logger)
        {
            _dockService = dockService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IReserveBike> context)
        {
            _logger.LogInformation($"Reserve bike to {context.CorrelationId} was received");
            
            try
            {
                var dockDto = await _dockService.GetByBikeId(context.Message.Rental.BikeId);

                context.Message.Rental.OriginDockId = dockDto.Id;
                
                await DetachBikeFromDock(dockDto);
                
                UpdateRentalState(context.Message.Rental, RentalStatus.BikeReserved);

                await SendBikeReserved(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating bike status");

                UpdateRentalState(context.Message.Rental, RentalStatus.RentalFailure);                

                await SendBikeReservationFailed(context);
            }
        }
        
        private async Task DetachBikeFromDock(DockDto dockDto)
        {
            dockDto.BikeId = null;

            await _dockService.UpdateAsync(dockDto.Id, dockDto);
        }
        
        private static async Task SendBikeReservationFailed(ConsumeContext<IReserveBike> context)
        {
            await context.RespondAsync<IRentalFailure>(new
            {
                context.CorrelationId,
                context.Message.Rental
            });
        }

        private static async Task SendBikeReserved(ConsumeContext<IReserveBike> context)
        {
            await context.RespondAsync<IBikeReserved>(new
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
}
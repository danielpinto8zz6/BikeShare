using Common.Commands;
using Common.Events;
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
            _logger.LogInformation($"Reserve bike to {context.Message.CorrelationId} was received");

            var message = context.Message;

            var isBikeDetachedFromDock = false;

            try
            {
                // var dockDto = await _dockService.GetByIdAsync(message.Rental.DockId);

                // isBikeDetachedFromDock = await DetachBikeFromDock(dockDto);
                isBikeDetachedFromDock = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating bike status");

                await SendBikeReservationFailed(context);
            }

            if (isBikeDetachedFromDock)
            {
                await SendBikeReserved(context);
            }
            else
            {
                await SendBikeReservationFailed(context);
            }
        }
        
        private async Task<bool> DetachBikeFromDock(DockDto dockDto)
        {
            if (dockDto.BikeId == null) return false;

            dockDto.BikeId = null;

            await _dockService.UpdateAsync(dockDto.Id, dockDto);

            return true;
        }
        
        private static async Task SendBikeReservationFailed(ConsumeContext<IReserveBike> context)
        {
            await context.Publish<IBikeReservationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.Rental
            });
        }

        private static async Task SendBikeReserved(ConsumeContext<IReserveBike> context)
        {
            await context.Publish<IBikeReserved>(new
            {
                context.Message.CorrelationId,
                context.Message.Rental
            });
        }
    }
}
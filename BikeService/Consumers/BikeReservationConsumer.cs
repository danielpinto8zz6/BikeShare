using System;
using System.Threading.Tasks;
using BikeService.Services;
using Common.Commands;
using Common.Enums;
using Common.Events;
using Common.Models.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BikeService.Consumers
{
    public class BikeReservationConsumer : IConsumer<IReserveBike>
    {
        private readonly ILogger<BikeReservationConsumer> _logger;

        private readonly IBikeService _bikeService;

        public BikeReservationConsumer(IBikeService bikeService, ILogger<BikeReservationConsumer> logger)
        {
            _bikeService = bikeService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IReserveBike> context)
        {
            _logger.LogInformation($"Reserve bike to {context.Message.CorrelationId} was received");

            var message = context.Message;

            var isBikeStatusUpdated = false;

            try
            {
                var bikeDto = await _bikeService.GetByIdAsync(message.Rental.BikeId);

                isBikeStatusUpdated = await HandleBikeStatusAsync(bikeDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating bike status");

                await SendBikeReservationFailed(context);
            }

            if (isBikeStatusUpdated)
            {
                await SendBikeReserved(context);
            }
            else
            {
                await SendBikeReservationFailed(context);
            }
        }
        
        private async Task<bool> HandleBikeStatusAsync(BikeDto bikeDto)
        {
            if (bikeDto.Status != BikeStatus.Available) return false;

            bikeDto.Status = BikeStatus.InUse;

            await _bikeService.UpdateAsync(bikeDto.Id, bikeDto);

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
            await context.Publish<IBikeReservationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.Rental
            });
        }
    }
}
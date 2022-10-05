using System;
using System.Threading.Tasks;
using BikeService.Services;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events.Rental;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BikeService.Consumers
{
    public class ValidateBikeConsumer : IConsumer<IValidateBike>
    {
        private readonly IBikeService _bikeService;

        private readonly ILogger<ValidateBikeConsumer> _logger;

        public ValidateBikeConsumer(
            ILogger<ValidateBikeConsumer> logger, IBikeService bikeService)
        {
            _logger = logger;
            _bikeService = bikeService;
        }

        public async Task Consume(ConsumeContext<IValidateBike> context)
        {
            _logger.LogInformation($"Validate bike to {context.CorrelationId} was received");
            
            var isBikeValid = await _bikeService.ExistAsync(context.Message.Rental.BikeId);

            UpdateRentalState(context.Message.Rental, isBikeValid ? RentalStatus.BikeValidated : RentalStatus.RentalFailed);

            if (isBikeValid)
            {
                _logger.LogInformation($"Bike validated for {context.CorrelationId}!");
                await SendBikeValidatedAsync(context);
            }
            else
            {
                _logger.LogInformation($"Bike invalid for {context.CorrelationId}!");
                await SendRentalFailedAsync(context);
            }
        }

        private async Task SendBikeValidatedAsync(ConsumeContext<IValidateBike> context)
        {
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{nameof(IBikeValidated)}"));
            await endpoint.Send<IBikeValidated>(new
            {
                context.CorrelationId,
                context.Message.Rental
            });
        }
        
        private async Task SendRentalFailedAsync(ConsumeContext<IValidateBike> context)
        {
            var endpoint = await context.GetSendEndpoint(new Uri($"queue:{nameof(IRentalFailed)}"));
            await endpoint.Send<IRentalFailed>(new
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
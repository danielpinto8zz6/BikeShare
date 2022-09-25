using System.Threading.Tasks;
using BikeService.Services;
using Common.Extensions.Exceptions;
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

            bool isBikeValid;
            try
            {
                var bike = await _bikeService.GetByIdAsync(context.Message.Rental.BikeId);
                isBikeValid = bike != null;
            }
            catch (NotFoundException e)
            {
                _logger.LogError($"Bike {context.Message.Rental.BikeId} not found in database!");

                isBikeValid = false;
            }

            UpdateRentalState(
                context.Message.Rental,
                isBikeValid ? RentalStatus.BikeValidated : RentalStatus.RentalFailure);

            if (isBikeValid)
            {
                _logger.LogInformation($"Bike validated for {context.CorrelationId}!");

                await context.RespondAsync<IBikeValidated>(new
                {
                    context.CorrelationId,
                    context.Message.Rental
                });
            }

            else
            {
                _logger.LogInformation($"Bike invalid for {context.CorrelationId}!");

                await context.RespondAsync<IRentalFailure>(new
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
}
using System.Threading.Tasks;
using BikeValidateService.Services;
using Common.Commands;
using Common.Enums;
using Common.Events;
using Common.Models.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BikeValidateService.Consumers
{
    public class ValidateBikeConsumer : IConsumer<IValidateBike>
    {
        // private readonly IBikeValidateService _bikeValidateService;

        private readonly ILogger<ValidateBikeConsumer> _logger;

        public ValidateBikeConsumer(
            // IBikeValidateService bikeValidateService,
            ILogger<ValidateBikeConsumer> logger
        )
        {
            // _bikeValidateService = bikeValidateService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IValidateBike> context)
        {
            _logger.LogInformation($"Validate bike to {context.Message.CorrelationId} was received");

            // var isBikeValid = await _bikeValidateService.IsBikeValidAsync(context.Message.Rental);
            var isBikeValid = true;

            UpdateRentalState(
                context.Message.Rental,
                isBikeValid ? RentalStatus.BikeValidated : RentalStatus.BikeValidationFailed);

            if (isBikeValid)
                await context.Publish<IBikeValidated>(new
                {
                    context.Message.CorrelationId,
                    context.Message.Rental
                });
            else
                await context.Publish<IBikeValidationFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.Rental
                });
        }

        private static void UpdateRentalState(RentalDto rentalDto, RentalStatus status)
        {
            rentalDto.Status = status;
        }
    }
}
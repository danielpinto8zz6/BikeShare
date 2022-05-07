using System.Threading.Tasks;
using Common.Models.Commands;
using Common.Models.Commands.Rental;
using Common.Models.Dtos;
using Common.Models.Enums;
using Common.Models.Events;
using Common.Models.Events.Rental;
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
            _logger.LogInformation($"Validate bike to {context.CorrelationId} was received");

            // var isBikeValid = await _bikeValidateService.IsBikeValidAsync(context.Message.Rental);
            var isBikeValid = true;

            UpdateRentalState(
                context.Message.Rental,
                isBikeValid ? RentalStatus.BikeValidated : RentalStatus.BikeValidationFailed);

            if (isBikeValid)
                await context.Publish<IBikeValidated>(new
                {
                    context.CorrelationId,
                    context.Message.Rental
                });
            else
                await context.Publish<IBikeValidationFailed>(new
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
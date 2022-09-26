using Common.Models.Commands.Rental;
using DockService.Services;
using MassTransit;

namespace DockService.Consumers
{
    public class BikeUnlockConsumer : IConsumer<IUnlockBike>
    {
        private readonly ILogger<BikeUnlockConsumer> _logger;

        private readonly IDockManagerService _dockManagerService;


        public BikeUnlockConsumer(
            ILogger<BikeUnlockConsumer> logger, 
            IDockManagerService dockManagerService)
        {
            _logger = logger;
            _dockManagerService = dockManagerService;
        }

        public Task Consume(ConsumeContext<IUnlockBike> context)
        {
            _logger.LogInformation($"Unlock bike to {context.CorrelationId} was received");

            return _dockManagerService.UnlockBikeAsync(context.Message);
        }
    }
}
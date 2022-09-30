using DockService.Models.Dtos;
using DockService.Services;
using MassTransit;

namespace DockService.Consumers
{
    public class BikeLockConsumer : IConsumer<BikeLockRequest>
    {
        private readonly ILogger<BikeLockConsumer> _logger;

        private readonly IDockManagerService _dockManagerService;


        public BikeLockConsumer(
            ILogger<BikeLockConsumer> logger,
            IDockManagerService dockManagerService)
        {
            _logger = logger;
            _dockManagerService = dockManagerService;
        }

        public Task Consume(ConsumeContext<BikeLockRequest> context)
        {
            _logger.LogInformation(
                $"Received lock request for bike {context.Message.BikeId} on dock {context.Message.DockId}.");

            return _dockManagerService.LockBikeAsync(context.Message);
        }
    }
}
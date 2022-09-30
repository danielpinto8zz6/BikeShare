using Common.Models.Events.Rental;
using DockService.Models.Dtos;

namespace DockService.Services;

public interface IDockManagerService
{
    Task LockBikeAsync(BikeLockRequest bikeLockRequest);
    Task UnlockBikeAsync(IRentalMessage rentalMessage);
}
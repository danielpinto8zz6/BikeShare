using Common.Models.Events.Rental;
using DockService.Models.Dtos;

namespace DockService.Services;

public interface IDockManagerService
{
    Task LockBikeAsync(BikeLockRequestDto bikeLockRequestDto);
    Task UnlockBikeAsync(IRentalMessage rentalMessage);
}
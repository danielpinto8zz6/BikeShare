using DockInternalServiceEmulator.Models.Dtos;

namespace DockInternalServiceEmulator.Services;

public interface IDockInternalService
{
    Task AttachBikeAsync(BikeAttachRequestDto bikeAttachRequestDto);
}
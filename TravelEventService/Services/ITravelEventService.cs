using Common.Models.Dtos;

namespace TravelEventService.Services;

public interface ITravelEventService
{
    Task CreateAsync(TravelEventDto travelEventDto);
}
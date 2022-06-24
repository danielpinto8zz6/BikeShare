using Common.Models.Dtos;

namespace TravelEventService.Services;

public interface ITravelEventService
{
    Task<TravelEventDto> CreateAsync(TravelEventDto travelEventDto);
    Task<IEnumerable<TravelEventDto>> GetByRentalIdAsync(Guid rentalId);
}
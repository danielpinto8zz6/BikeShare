using Common.Models.Dtos;

namespace Common.TravelEvent.Services;

public interface ITravelEventService
{
    Task<TravelEventDto> CreateAsync(TravelEventDto travelEventDto);
    Task<IEnumerable<TravelEventDto>> GetByRentalIdAsync(Guid rentalId);
}
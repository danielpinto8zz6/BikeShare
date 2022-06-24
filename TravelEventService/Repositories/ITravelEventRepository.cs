using Common.Services.Repositories;
using TravelEventService.Entities;

namespace TravelEventService.Repositories;

public interface ITravelEventRepository : IMongoDbRepository
{
    Task<IEnumerable<TravelEvent>> GetByRentalIdAsync(Guid rentalId);
}
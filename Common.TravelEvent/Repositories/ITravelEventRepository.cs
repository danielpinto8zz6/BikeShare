using Common.Services.Repositories;

namespace Common.TravelEvent.Repositories;

public interface ITravelEventRepository : IMongoDbRepository
{
    Task<IEnumerable<Entities.TravelEvent>> GetByRentalIdAsync(Guid rentalId);
}
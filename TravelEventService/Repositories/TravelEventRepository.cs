using Common.Services.Repositories;
using MongoDB.Driver;
using TravelEventService.Entities;

namespace TravelEventService.Repositories;

public class TravelEventRepository : MongoDbRepository, ITravelEventRepository
{
    private readonly IMongoDatabase _mongoDatabase;

    public TravelEventRepository(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
    {
        _mongoDatabase = mongoClient.GetDatabase(databaseName);
    }

    public async Task<IEnumerable<TravelEvent>> GetByRentalIdAsync(Guid rentalId)
    {
        var mongoCollection = _mongoDatabase.GetCollection<TravelEvent>(nameof(TravelEvent));

        var filter = Builders<TravelEvent>.Filter.Eq(travelEvent => travelEvent.RentalId, rentalId);

        return await mongoCollection
            .Find(filter)
            .SortBy(travelEvent => travelEvent.CreatedDate)
            .ToListAsync();
    }
}
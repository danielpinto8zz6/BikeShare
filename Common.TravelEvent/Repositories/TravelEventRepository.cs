using Common.Services.Repositories;
using MongoDB.Driver;

namespace Common.TravelEvent.Repositories;

public class TravelEventRepository : MongoDbRepository, ITravelEventRepository
{
    private readonly IMongoDatabase _mongoDatabase;

    public TravelEventRepository(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
    {
        _mongoDatabase = mongoClient.GetDatabase(databaseName);
    }

    public async Task<IEnumerable<Entities.TravelEvent>> GetByRentalIdAsync(Guid rentalId)
    {
        var mongoCollection = _mongoDatabase.GetCollection<Entities.TravelEvent>(nameof(Entities.TravelEvent));

        var filter = Builders<Entities.TravelEvent>.Filter.Eq(travelEvent => travelEvent.RentalId, rentalId);

        return await mongoCollection
            .Find(filter)
            .SortBy(travelEvent => travelEvent.CreatedDate)
            .ToListAsync();
    }
}
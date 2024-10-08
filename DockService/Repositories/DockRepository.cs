using Common.Services.Repositories;
using DockService.Models.Dtos;
using DockService.Models.Entities;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace DockService.Repositories
{
    public class DockRepository : MongoDbRepository, IDockRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public DockRepository(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
        {
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
        }

        public async Task<IQueryable<Dock>> GetNearByDocksAsync(NearByDocksRequestDto nearByDocksRequestDto)
        {
            if (nearByDocksRequestDto.Coordinates == null)
            {
                return new List<Dock>().AsQueryable();
            }

            var collection = _mongoDatabase.GetCollection<Dock>(nameof(Dock));
            var point = GeoJson.Point(new GeoJson2DGeographicCoordinates(nearByDocksRequestDto.Coordinates.Longitude,
                nearByDocksRequestDto.Coordinates.Latitude));
            var filter =
                Builders<Dock>.Filter.Near(dock => dock.Coordinates, point, nearByDocksRequestDto.Radius * 1000);

            var result = await collection.FindAsync(filter);
            var bikes = await result.ToListAsync();

            return bikes.AsQueryable();
        }

        public Task<Dock> GetByBikeId(Guid bikeId)
        {
            var mongoCollection = _mongoDatabase.GetCollection<Dock>(nameof(Dock));

            var filter = Builders<Dock>.Filter.Eq(dock => dock.BikeId, bikeId);

            return mongoCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
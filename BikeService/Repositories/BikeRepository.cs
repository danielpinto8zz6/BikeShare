using System.Linq;
using System.Threading.Tasks;
using BikeService.Models.Dtos;
using BikeService.Models.Entities;
using Common.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BikeService.Repositories
{
    public class BikeRepository : MongoDbRepository, IBikeRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public BikeRepository(IMongoClient mongoClient, string databaseName) : base(mongoClient, databaseName)
        {
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
        }

        public async Task<IQueryable<Bike>> GetNearByAsync(NearByBikesRequestDto nearByBikesRequestDto)
        {
            var collection = _mongoDatabase.GetCollection<Bike>(nameof(Bike));
            var point = GeoJson.Point(new GeoJson2DGeographicCoordinates(nearByBikesRequestDto.Coordinates.Longitude,
                nearByBikesRequestDto.Coordinates.Latitude));
            var filter =
                Builders<Bike>.Filter.Near(bike => bike.Coordinates, point, nearByBikesRequestDto.Radius * 1000);

            var result = await collection.FindAsync(filter);
            var bikes = await result.ToListAsync();

            return bikes.AsQueryable();
        }
    }
}
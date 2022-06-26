using Common.Models.Dtos;
using MongoDB.Driver;

namespace Common.Services.Repositories
{
    public class MongoDbRepository : IMongoDbRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbRepository(IMongoClient mongoClient, string databaseName)
        {
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
        }

        public async Task<IQueryable<T2>> GetAllAsync<T1, T2>() where T2 : class, IBaseEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var cursor = await mongoCollection.FindAsync(_ => true);
            var results = await cursor.ToListAsync();

            return results.AsQueryable();
        }

        public async Task<T2> GetByIdAsync<T1, T2>(T1 id) where T2 : class, IBaseEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq("_id", id);

            var cursor = await mongoCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<T2> CreateAsync<T1, T2>(T2 entity) where T2 : class, IBaseEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            entity.CreatedDate = DateTime.UtcNow;

            await mongoCollection.InsertOneAsync(entity);

            return entity;
        }

        public async Task<T2> DeleteAsync<T1, T2>(T1 id) where T2 : class, IBaseEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq("_id", id);

            var cursor = await mongoCollection.FindAsync(filter);
            var result = await cursor.FirstOrDefaultAsync();

            await mongoCollection.DeleteOneAsync(filter);

            return result;
        }

        public async Task<T2> UpdateAsync<T1, T2>(T1 id, T2 entity) where T2 : class, IBaseEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            entity.ModifiedDate = DateTime.UtcNow;

            var filter = Builders<T2>.Filter.Eq(s => s.Id, id);
            await mongoCollection.ReplaceOneAsync(filter, entity);

            return await GetByIdAsync<T1, T2>(id);
        }
    }
}
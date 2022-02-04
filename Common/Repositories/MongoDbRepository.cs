using System;
using System.Linq;
using System.Threading.Tasks;
using LSG.GenericCrud.Models;
using MongoDB.Driver;

namespace Common.Repositories
{
    public class MongoDbRepository : IMongoDbRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbRepository(IMongoClient mongoClient, string databaseName)
        {
            _mongoDatabase = mongoClient.GetDatabase(databaseName);
        }

        public IQueryable<T2> GetAll<T1, T2>() where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var result = mongoCollection.Find(_ => true).ToList();

            return result.AsQueryable();
        }

        public async Task<IQueryable<T2>> GetAllAsync<T1, T2>() where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var result = await mongoCollection.Find(_ => true).ToListAsync();

            return result.AsQueryable();
        }

        public T2 GetById<T1, T2>(T1 id) where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq("_id", id);

            return mongoCollection.Find(filter).FirstOrDefault();
        }

        public async Task<T2> GetByIdAsync<T1, T2>(T1 id) where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq("_id", id);

            return await mongoCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T2> CreateAsync<T1, T2>(T2 entity) where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            await mongoCollection.InsertOneAsync(entity);

            return entity;
        }

        public T2 Delete<T1, T2>(T1 id) where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq("_id", id);

            var result = mongoCollection.Find(filter).FirstOrDefault();

            mongoCollection.DeleteOne(filter);

            return result;
        }

        public async Task<T2> DeleteAsync<T1, T2>(T1 id) where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq("_id", id);

            var result = await mongoCollection.Find(filter).FirstOrDefaultAsync();

            await mongoCollection.DeleteOneAsync(filter);

            return result;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }

        public async Task<T2> UpdateAsync<T1, T2>(T1 id, T2 entity) where T2 : class, IEntity<T1>, new()
        {
            var mongoCollection = _mongoDatabase.GetCollection<T2>(typeof(T2).Name);

            var filter = Builders<T2>.Filter.Eq(s => s.Id, id);
            await mongoCollection.ReplaceOneAsync(filter, entity);

            return await GetByIdAsync<T1, T2>(id);
        }
    }
}